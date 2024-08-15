using System.Runtime.InteropServices;
using Celebratix.Common.Configs;
using Celebratix.Common.Extensions;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs.User;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Event = Stripe.Event;

namespace Celebratix.Common.Services;

/// <summary>
/// I.e. Stripe service
/// </summary>
public class PaymentService
{
    private readonly CelebratixDbContext _dbContext;
    private readonly StripeConfig _stripeConfig;
    private readonly MarketplaceConfig _marketplaceConfig;
    private readonly ILogger<PaymentService> _logger;

    private readonly Stripe.AccountService _stripeAccountService;

    public PaymentService(CelebratixDbContext dbContext, ILogger<PaymentService> logger,
        IOptions<StripeConfig> stripeConfig, IOptions<MarketplaceConfig> marketplaceConfig)
    {
        _stripeAccountService = new Stripe.AccountService();
        _dbContext = dbContext;
        _logger = logger;
        _stripeConfig = stripeConfig.Value;
        _marketplaceConfig = marketplaceConfig.Value;
    }

    public void HandleStripeEvent(Event stripeEvent)
    {
        if (stripeEvent.Type == Events.PaymentIntentRequiresAction)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            _logger.LogInformation("Payment for {PaymentIntentId} is requiring user action (Stripe)", paymentIntent!.Id);
            BackgroundJob.Enqueue<OrderService>(x =>
                x.SetOrderToRequiresUserAction(paymentIntent.Id)
            );
        }
        else if (stripeEvent.Type == Events.PaymentIntentProcessing)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            _logger.LogInformation("Payment for {PaymentIntentId} is processing (Stripe)", paymentIntent!.Id);
            BackgroundJob.Enqueue<OrderService>(x =>
                x.SetOrderToProcessing(paymentIntent.Id)
            );
        }
        else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            _logger.LogInformation("A successful payment for {PaymentIntentId} was made (Stripe). Amount {PaymentIntentAmount}",
                paymentIntent!.Id, paymentIntent.Amount);
            BackgroundJob.Enqueue<OrderService>(x =>
                x.FulfillOrder(paymentIntent.Id)
            );
        }
        else if (stripeEvent.Type == Events.PaymentIntentCanceled)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            _logger.LogInformation("Payment for {PaymentIntentId} was cancelled (Stripe)", paymentIntent!.Id);
        }
        else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            _logger.LogInformation("Payment for {PaymentIntentId} failed (Stripe)", paymentIntent!.Id);
            BackgroundJob.Enqueue<OrderService>(x =>
                // We do not want to fail (cancel) the order here, because we allow the intent to be re-used and for the user to try again
                x.SetOrderToAwaitingPaymentInfo(paymentIntent.Id)
            );
        }
        else if (stripeEvent.Type == Events.AccountUpdated)
        {
            var account = stripeEvent.Data.Object as Account;
            _logger.LogInformation("An account update event received for account with id {AccountId}", account!.Id);
            BackgroundJob.Enqueue<PaymentService>(x =>
                x.UpdateAccountRequirementStatus(account)
            );
        }
        // else if (stripeEvent.Type == Events.PaymentMethodAttached)
        // {
        //     var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
        //     // Then define and call a method to handle the successful attachment of a PaymentMethod.
        //     // handlePaymentMethodAttached(paymentMethod);
        // }
        else
        {
            _logger.LogInformation("Unhandled Stripe event. Type: {StripeEventType}", stripeEvent!.Type);
        }
    }

    public async Task<string> GetOrCreateStripeCustomerId(string userId)
    {
        var user = await _dbContext.Users.FirstOrThrowAsync(u => u.Id == userId);

        if (user.StripeCustomerId != null)
        {
            return user.StripeCustomerId;
        }

        var options = new CustomerCreateOptions
        {
            Description = user.Id,
            Phone = user.PhoneNumber?.Trim(),
            Email = user.Email?.Trim(),
            Name = user.FullName.Trim()
        };
        var service = new CustomerService();
        var stripeCustomer = await service.CreateAsync(options);

        user.StripeCustomerId = stripeCustomer.Id;
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Successfully created stripe customer with id: {CustomerId}", stripeCustomer.Id);

        return stripeCustomer.Id;
    }

    /// <summary>
    /// If no stripe customer exists on the user, then just null is returned
    /// </summary>
    public async Task<string?> UpdateStripeCustomer(ApplicationUser user)
    {
        if (user.StripeCustomerId == null)
            return null;

        var options = new CustomerUpdateOptions()
        {
            Phone = user.PhoneNumber?.Trim(),
            Email = user.Email?.Trim(),
            Name = user.FullName.Trim()
        };
        var service = new CustomerService();
        var stripeCustomer = await service.UpdateAsync(user.StripeCustomerId, options);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Successfully updated stripe customer with id: {CustomerId}", stripeCustomer.Id);

        return stripeCustomer.Id;
    }

    public async Task<EphemeralKey> GetEphemeralKeyForStripeCustomer(string stripeCustomerId)
    {
        var options = new EphemeralKeyCreateOptions
        {
            Customer = stripeCustomerId
        };

        var service = new EphemeralKeyService();
        var ephemeralKey = await service.CreateAsync(options);
        return ephemeralKey;
    }

    public async Task<PaymentIntent> CreateStripePaymentIntent(string stripeCustomerId, decimal amount,
        Currency currency, string? sellerAccountId = null)
    {

        var stripeAmount = Convert.ToInt64(amount * (decimal)Math.Pow(10, currency.DecimalPlaces));
        var options = new PaymentIntentCreateOptions
        {
            Customer = stripeCustomerId,
            Amount = stripeAmount,
            Currency = currency.Code.ToLower(),
            //ReceiptEmail = userEmail,
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            },
            // Confirm = true,
            // ReturnUrl = _stripeConfig.PaymentReturnUrl
        };

        if (sellerAccountId != null)
        {
            var stripeServiceFee = Convert.ToInt64(Math.Round(stripeAmount * _marketplaceConfig.ServiceFeeFraction));
            options.TransferData = new PaymentIntentTransferDataOptions
            {
                Destination = sellerAccountId,
                Amount = stripeAmount - stripeServiceFee
            };
        }

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options);
        _logger.LogInformation("Successfully created stripe payment intent with id: {PaymentIntentId}", paymentIntent.Id);
        return paymentIntent;
    }

    public async Task CancelPaymentIntent(string paymentIntentId)
    {
        var service = new PaymentIntentService();
        try
        {
            await service.CancelAsync(paymentIntentId);
        }
        catch (StripeException e)
        {
            // We don't mind if the intent is already cancelled
            if (e.StripeError.Code != "payment_intent_unexpected_state") throw;
            _logger.LogWarning("Stripe intent was already cancelled. Intent id {PaymentIntentId}", paymentIntentId);
            return;
        }
        _logger.LogInformation("Successfully cancelled stripe payment intent with id: {PaymentIntentId}", paymentIntentId);
    }

    public async Task<StripeAccountLinkResponseDto> CreateStripeOnboardingAccountLink(string userId, string returnUrl, string refreshUrl)
    {
        var stripeAccountId = await GetOrCreateStripeAccountId(userId);

        var options = new AccountLinkCreateOptions
        {
            Account = stripeAccountId,
            ReturnUrl = returnUrl, // TODO: only provide path or have internal whitelist
            RefreshUrl = refreshUrl,
            Type = "account_onboarding",
            Collect = "currently_due",
        };
        var service = new AccountLinkService();
        var accountLink = await service.CreateAsync(options);

        return new StripeAccountLinkResponseDto
        {
            ExpiresAt = accountLink.ExpiresAt,
            Url = accountLink.Url
        };
    }

    public async Task<StripeAccountLinkResponseDto> CreateStripeUpdateAccountLink(string userId, string returnUrl, string refreshUrl)
    {
        var stripeAccountId = await GetOrCreateStripeAccountId(userId);

        var options = new AccountLinkCreateOptions
        {
            Account = stripeAccountId,
            ReturnUrl = returnUrl, // TODO: only provide path or have internal whitelist
            RefreshUrl = refreshUrl,
            Type = "account_update",
            Collect = "currently_due",
        };
        var service = new AccountLinkService();
        var accountLink = await service.CreateAsync(options);

        return new StripeAccountLinkResponseDto
        {
            ExpiresAt = accountLink.ExpiresAt,
            Url = accountLink.Url
        };
    }

    public async Task UpdateAccountRequirementStatus(Account stripeAccount)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.StripeConnectAccountId == stripeAccount.Id);

        if (user == null)
        {
            _logger.LogWarning("User with stripe connect account id {StripeAccountId} missing!", stripeAccount.Id);
            return;
        }

        var oldSubmittedStatus = user.StripePayoutRequirementsSubmitted;
        var oldFulfilledStatus = user.StripePayoutRequirementsFulfilled;

        user.StripePayoutRequirementsSubmitted = stripeAccount.DetailsSubmitted && !stripeAccount.Requirements.CurrentlyDue.Any();
        user.StripePayoutRequirementsFulfilled = stripeAccount.PayoutsEnabled;

        await _dbContext.SaveChangesAsync();

        if (oldSubmittedStatus != user.StripePayoutRequirementsSubmitted)
        {
            _logger.LogInformation("Stripe payout req. submitted status changed to: {NewStatus} for user: {UserId}",
                user.StripePayoutRequirementsSubmitted, user.Id);
        }

        if (oldFulfilledStatus != user.StripePayoutRequirementsFulfilled)
        {
            _logger.LogInformation("Stripe payout req. fulfilled status changed to: {NewStatus} for user: {UserId}",
                user.StripePayoutRequirementsFulfilled, user.Id);
        }
    }

    public async Task<string> GetOrCreateStripeAccountId(string userId)
    {
        var user = await _dbContext.Users.FirstOrThrowAsync(u => u.Id == userId);

        if (user.StripeConnectAccountId != null)
        {
            return user.StripeConnectAccountId;
        }

        var account = await CreateAccount(user);
        user.StripeConnectAccountId = account.Id;
        await _dbContext.SaveChangesAsync();

        return account.Id;
    }

    public async Task<Account> GetOrCreateStripeAccount(string userId, [Optional] CreateStripeConnectAccountDto? dto)
    {
        var user = await _dbContext.Users.FirstOrThrowAsync(u => u.Id == userId);

        if (user.StripeConnectAccountId != null)
        {
            return await GetAccount(user.StripeConnectAccountId);
        }

        var account = await CreateAccount(user, dto);

        user.StripeConnectAccountId = account.Id;
        await _dbContext.SaveChangesAsync();

        return account;
    }

    public async Task<Account> GetAccount(string id)
    {
        return await _stripeAccountService.GetAsync(id);
    }

    public async Task<Account> CreateAccount(ApplicationUser user, [Optional] CreateStripeConnectAccountDto? dto, [Optional] string? ipAddress)
    {
        var emailNormalized = user.Email?.Trim();
        var normalizedAddress = dto?.AddressLine1.Trim();
        var normalizedCity = dto?.City.Trim();
        var normalizedCountryCode = dto?.CountryCode.Trim().ToUpper();
        var normalizedPostalCode = dto?.PostalCode.Trim();
        var options = new AccountCreateOptions
        {
            Type = AccountType.Custom,
            Country = normalizedCountryCode,
            Metadata = new Dictionary<string, string>
            {
                {"id", user.Id}
            },
            BusinessType = "individual",
            Email = emailNormalized,
            Individual = new AccountIndividualOptions
            {
                Address = new AddressOptions
                {
                    City = normalizedCity,
                    Country = normalizedCountryCode,
                    PostalCode = normalizedPostalCode,
                    Line1 = normalizedAddress,
                },
                FirstName = dto?.FirstName.Trim(),
                LastName = dto?.LastName.Trim(),
                Email = emailNormalized,
                Phone = user.PhoneNumber?.Trim(),
                Gender = user.Gender switch
                {
                    Enums.Gender.Male => "male",
                    Enums.Gender.Female => "female",
                    Enums.Gender.Other => null,
                    _ => null
                },
                Dob = new DobOptions
                {
                    Day = dto?.DateOfBirth.Day,
                    Month = dto?.DateOfBirth.Month,
                    Year = dto?.DateOfBirth.Year
                }
            },
            BusinessProfile = new AccountBusinessProfileOptions
            {
                Url = "www.celebratix.io"
            },
            Capabilities = new AccountCapabilitiesOptions
            {
                // CardPayments = new AccountCapabilitiesCardPaymentsOptions
                // {
                //     Requested = true,
                // },
                Transfers = new AccountCapabilitiesTransfersOptions
                {
                    Requested = true,
                },
            },
            TosAcceptance = new AccountTosAcceptanceOptions
            {
                Date = DateTimeOffset.UtcNow.DateTime,
                Ip = ipAddress
            }
        };
        var account = await _stripeAccountService.CreateAsync(options);

        _logger.LogInformation("Successfully created stripe account with id: {AccountId}", account.Id);

        return account;
    }

    public async Task<PayoutAccount> RegisterBankAccountForUser(string userId, CreateStripeConnectAccountDto dto, [Optional] string ipAddress)
    {
        var normalizedAccountName = dto.AccountName.Trim();
        var normalizedAccountNumber = dto.AccountNumber.Trim();
        var normalizedAddress = dto.AddressLine1.Trim();
        var normalizedCity = dto.City.Trim();
        var normalizedCountryCode = dto.CountryCode.Trim().ToUpper();
        var normalizedCurrencyCode = dto.CurrencyCode.Trim().ToUpper();
        var normalizedPostalCode = dto.PostalCode.Trim();

        var user = await _dbContext.Users
            .Include(u => u.PayoutAccounts)
            .FirstOrThrowAsync(u => u.Id == userId);
        Account account;
        if (user.StripeConnectAccountId != null)
        {
            account = await GetAccount(user.StripeConnectAccountId);
        }
        else
        {
            account = await CreateAccount(user, dto, ipAddress);
            user.StripeConnectAccountId = account.Id;
            await _dbContext.SaveChangesAsync();
        }

        var service = new ExternalAccountService();
        await service.CreateAsync(account.Id, new ExternalAccountCreateOptions
        {
            ExternalAccount = new AccountBankAccountOptions
            {
                Country = account.Country,
                Currency = normalizedCurrencyCode,
                AccountHolderName = normalizedAccountName,
                AccountNumber = normalizedAccountNumber,
            },
            DefaultForCurrency = true
        });

        var payoutAccount = user.PayoutAccounts?.FirstOrDefault();
        var payoutAccounts = _dbContext.PayoutAccounts;
        if (payoutAccount == null)
        {
            payoutAccounts.Add(new PayoutAccount
            {
                OwnerId = userId,
                DateOfBirth = dto.DateOfBirth,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                AccountName = normalizedAccountName,
                AccountNumber = normalizedAccountNumber,
                AddressLine1 = normalizedAddress,
                CountryCode = normalizedCountryCode,
                City = normalizedCity,
                CurrencyCode = normalizedCurrencyCode,
                PostalCode = normalizedPostalCode,
                IpAddress = ipAddress,
            });
        }
        else
        {
            payoutAccounts.Update(new PayoutAccount
            {
                OwnerId = userId,
                DateOfBirth = dto.DateOfBirth,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                AccountName = normalizedAccountName,
                AccountNumber = normalizedAccountNumber,
                AddressLine1 = normalizedAddress,
                CountryCode = normalizedCountryCode,
                City = normalizedCity,
                CurrencyCode = normalizedCurrencyCode,
                PostalCode = normalizedPostalCode,
                IpAddress = ipAddress,
            });
        }
        _ = await _dbContext.SaveChangesAsync();

        return payoutAccount!;
    }
}
