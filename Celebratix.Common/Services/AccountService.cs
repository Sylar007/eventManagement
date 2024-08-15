using System.Data;
using Celebratix.Common.Configs;
using Celebratix.Common.Exceptions;
using Celebratix.Common.Extensions;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhoneNumbers;
using Twilio.Rest.Verify.V2.Service;

namespace Celebratix.Common.Services;

public class AccountService
{
    private readonly string DUMMY_PHONE_NUMBER = "+15551234567";

    private readonly string DUMMY_OTP_CODE = "123456";

    private readonly UserService _userService;
    private readonly TwilioVerifySettings _twilioVerifySettings;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly CelebratixDbContext _dbContext;
    private readonly PaymentService _paymentService;
    private readonly ILogger<AccountService> _logger;

    public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        CelebratixDbContext dbContext, ILogger<AccountService> logger, UserService userService,
        IOptions<TwilioVerifySettings> twilioVerifySettings, PaymentService paymentService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _dbContext = dbContext;
        _logger = logger;
        _userService = userService;
        _paymentService = paymentService;
        _twilioVerifySettings = twilioVerifySettings.Value;
    }

    public async Task InitializeLoginWithPhone(string rawPhoneNumber)
    {
        var phoneUtil = PhoneNumberUtil.GetInstance();
        var parsedNumber = phoneUtil.Parse(rawPhoneNumber.Trim(), null);
        var phoneNumber = phoneUtil.Format(parsedNumber, PhoneNumberFormat.E164);

        if (phoneNumber == DUMMY_PHONE_NUMBER)
        {
            return;
        }

        var verification = await VerificationResource.CreateAsync(
            to: phoneNumber,
            channel: "sms",
            pathServiceSid: _twilioVerifySettings.VerificationServiceSID
        //deviceIp:
        //locale:
        //customMessage:
        );

        if (verification.Status != "pending")
        {
            _logger.LogInformation("Error sending verification SMS with Twilio for phone: {Phone}", phoneNumber);
            throw new SmsSendingFailedException();
        }

        _logger.LogInformation("Successfully sent SMS sign in request to Twilio for phone: {Phone}", phoneNumber);
    }

    public async Task<(SignInResult, UserDto?)> LoginWithPhone(string rawPhoneNumber, string verificationCode, bool staySignedIn)
    {
        var phoneUtil = PhoneNumberUtil.GetInstance();
        var parsedNumber = phoneUtil.Parse(rawPhoneNumber.Trim(), null);
        var phoneNumber = phoneUtil.Format(parsedNumber, PhoneNumberFormat.E164);

        // TODO: move these hardcoded values into the config
        var isApproved = phoneNumber == DUMMY_PHONE_NUMBER && verificationCode == DUMMY_OTP_CODE;
        if (!isApproved)
        {
            var verification = await VerificationCheckResource.CreateAsync(
                to: phoneNumber,
                code: verificationCode,
                pathServiceSid: _twilioVerifySettings.VerificationServiceSID
            );
            isApproved = verification.Status == "approved";
        }

        if (!isApproved)
        {
            return (SignInResult.Failed, null);
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

        // If no user exists we create a new account
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = phoneNumber,
                PhoneNumber = phoneNumber,
                PhoneNumberConfirmed = true,
                CreatedAt = DateTimeOffset.UtcNow,
            };

            var result = await _userManager.CreateAsync(user);

            // We send email verification email if the registration succeeded
            if (result.Succeeded)
            {
                _logger.LogInformation("New user created, Id: {UserId}, Phone: {Phone}",
                    user.Id, user.PhoneNumber);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Code));
                _logger.LogInformation("User registration attempt failed, Phone: {Phone}\n" +
                                       "Error codes: {Errors}", phoneNumber, errors);
                throw new Exception("Account creation failed"); // Internal server error
            }
        }

        user.PhoneNumberConfirmed = true;

        // The login method performs an update to the user, saving the "phoneNumberConfrimed"
        await LoginWithoutPassword(user, staySignedIn);

        _logger.LogInformation("User logged in, Phone: {Phone}", phoneNumber);

        // Maybe unnecessary to fetch the user again but it's also easier to prevent bugs if we are consistent with what is returned.
        return (SignInResult.Success, await _userService.GetUserDtoById(user.Id));
    }

    public async Task<(SignInResult, UserDto?)> LoginWithEmail(string email, string password, bool staySignedIn)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return (SignInResult.Failed, null);
        }

        if (user.EmailConfirmed == false)
        {
            throw new EmailNotConfirmedException();
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, staySignedIn, false);

        if (result.Succeeded)
        {
            user.LastLoggedIn = DateTimeOffset.UtcNow;
            await _userManager.UpdateAsync(user);
            _logger.LogInformation("User logged in, Email: {Email}", email);
        }
        else
        {
            _logger.LogInformation("User login attempt failed, Email: {Email}", email);
        }

        // Maybe unnecessary to fetch the user again but it's also easier to prevent bugs if we are consistent with what is returned.
        return (result, await _userService.GetUserDtoById(user.Id));
    }

    /// <summary>
    /// This is meant to be used eg. after signing up a new user or after the user has reset its password
    /// </summary>
    public async Task LoginWithoutPassword(ApplicationUser user, bool staySignedIn)
    {
        _ = user ?? throw new ArgumentNullException(nameof(user));

        user.LastLoggedIn = DateTimeOffset.UtcNow;
        await _userManager.UpdateAsync(user);
        // await _signInManager.SignInAsync(user, staySignedIn);

        _logger.LogInformation("User logged (without password) in, Email: {UserEmail}", user.Email);
    }

    public async Task Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User signed out"); //
    }

    /// <summary>
    /// Refreshes the request senders cookie with the user specified in the id
    /// Note: This can't be used to refresh "someone else's" cookie. I.e. calling this from the admin panel is pointless
    /// </summary>
    public async Task RefreshCookie(string userId)
    {
        var user = await _userManager.Users.FirstOrThrowAsync(u => u.Id == userId);

        await _signInManager.RefreshSignInAsync(user);
        _logger.LogInformation("User refreshed (without password) in, Email: {UserEmail}", user.Email);
    }

    public async Task<RefreshToken> GenerateRefreshToken(string userId, DateTime expires, Guid? originator = null)
    {
        Guid tokenId = Guid.NewGuid();

        var rt = new RefreshToken
        {
            Id = tokenId,
            Originator = originator ?? tokenId,
            UserId = userId,
            Expires = expires,
        };
        _dbContext.RefreshTokens.Add(rt);
        await _dbContext.SaveChangesAsync();
        return rt;
    }

    public async Task<RefreshToken> DescendRefreshToken(RefreshToken oldRefreshToken, DateTime expires)
    {
        _dbContext.RefreshTokens.Remove(oldRefreshToken); // Delete is executed on save in GenerateRefreshToken
        var newRefreshToken = await GenerateRefreshToken(oldRefreshToken.UserId, expires, oldRefreshToken.Originator);
        return newRefreshToken;
    }

    public async Task InvalidateRefreshTokenFamily(Guid originator)
    {
        await _dbContext.RefreshTokens
            .Where(rt => rt.Originator == originator)
            .ExecuteDeleteAsync();
        // await _dbContext.SaveChangesAsync(); // Not needed, ExecuteDeleteAsync already saves
    }

    public async Task<RefreshToken> GetRefreshToken(Guid refreshTokenId)
    {
        return await _dbContext.RefreshTokens
            .Where(rt => rt.Id == refreshTokenId)
            .FirstOrThrowAsync();
    }

    /// <summary>
    /// Tries to confirm the email of the user with the given id (by using the token).
    /// </summary>
    /// <exception cref="ObjectNotFoundException">If not user with the given id was found</exception>
    public async Task<(IdentityResult, ApplicationUser)> ActivateAccount(string email, string newPassword, string emailConfirmationToken)
    {
        var normalizeEmail = _userManager.NormalizeEmail(email);
        var user = await _userManager.Users.FirstOrThrowAsync(u => u.NormalizedEmail == normalizeEmail);

        if (user.EmailConfirmed)
        {
            throw new AccountAlreadyActivatedException();
        }

        var result = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);

        if (result.Succeeded)
        {
            _logger.LogInformation("User successfully confirmed email: {UserEmail}", user.Email);
            await _userManager.AddPasswordAsync(user, newPassword);
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Code));
            _logger.LogInformation(
                "Confirmation of email failed for user with email: {UserEmail}\nError codes: {Errors}",
                user.Email, errors);
        }

        return (result, user);
    }


    /// <summary>
    /// Tries to confirm the email of the user with the given id (by using the token).
    /// </summary>
    /// <exception cref="ObjectNotFoundException">If not user with the given id was found</exception>
    public async Task<IdentityResult> ConfirmEmail(string email, string token)
    {
        var normalizeEmail = _userManager.NormalizeEmail(email);
        var user = await _userManager.Users.FirstOrThrowAsync(u => u.NormalizedEmail == normalizeEmail);

        if (user.EmailConfirmed)
        {
            throw new EmailAlreadyConfirmedException();
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
        {
            _logger.LogInformation("User successfully confirmed email: {UserEmail}", user.Email);
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Code));
            _logger.LogInformation(
                "Confirmation of email failed for user with email: {UserEmail}\nError codes: {Errors}",
                user.Email, errors);
        }

        return result;
    }

    public async Task<IdentityResult> UpdateAccountEmail(string userId, UpdateAccountEmailDto dto)
    {
        var trimmedEmail = dto.Email.Trim();
        var normalizedEmail = _userManager.NormalizeEmail(trimmedEmail);

        var user = await _dbContext.Users.FirstOrThrowAsync(u => u.Id == userId);

        if (user.EmailConfirmed)
        {
            throw new EmailAlreadyConfirmedException();
        }

        if (user.NormalizedEmail == normalizedEmail)
        {
            throw new ConstraintException("Cannot change email to the same email " + trimmedEmail);
        }

        await _userService.VerifyEmailIsFree(normalizedEmail);
        user.Email = trimmedEmail;
        var result = await _userManager.SetEmailAsync(user, trimmedEmail);
        //await RequestEmailConfirmationEmail(user.Id); // Disabled for now
        await _paymentService.UpdateStripeCustomer(user);

        return result;
    }

    public async Task RequestEmailConfirmationEmail(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new ObjectNotFoundException($"No user with the id {userId} was found!");
        }

        if (!user.EmailConfirmed)
        {
            await SendEmailConfirmationEmail(user);
        }
        else
        {
            throw new InvalidOperationException("Email already confirmed");
        }
    }

    private async Task SendEmailConfirmationEmail(ApplicationUser user)
    {
        // Consider adding cooldown for requesting confirm emails here
        var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        _logger.LogInformation("Generated email confirm (email confirm.) token, Email: {UserEmail}", user.Email);
        NotificationService.SendEmailConfirmationWithHangfire(user, confirmEmailToken);
    }

    public async Task<UserDto> SetAccountDetails(string userId, AccountDetailsUpdateDto updateDto)
    {
        var user = await _dbContext.Users.FirstOrThrowAsync(u => u.Id == userId);

        user.FirstName = updateDto.FirstName.Trim();
        user.LastName = updateDto.LastName.Trim();
        user.Gender = updateDto.Gender;
        user.DateOfBirth = updateDto.DateOfBirth;

        await _dbContext.SaveChangesAsync();
        await _paymentService.UpdateStripeCustomer(user);

        _logger.LogInformation("User with ID: {UserId} updated account details", user.Id);

        // Maybe unnecessary to fetch the user again but it's also easier to prevent bugs if we are consistent with what is returned.
        return await _userService.GetUserDtoById(user.Id);
    }

    /// <summary>
    /// Verifies (checks) that the supplied password is the current password for the given user
    /// </summary>
    /// <exception cref="ObjectNotFoundException">If not user with the given id was found</exception>
    public async Task<bool> VerifyPassword(string userId, string password)
    {
        var user = await _userManager.Users.FirstOrThrowAsync(u => u.Id == userId);

        var result = await _userManager.CheckPasswordAsync(user, password);

        return result;
    }

    /// <exception cref="ObjectNotFoundException">If not user with the given email was found</exception>
    public async Task RequestPasswordReset(ForgotPasswordDto forgotPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email.Trim());

        if (user == null)
        {
            throw new ObjectNotFoundException($"No user with the email {forgotPasswordDto.Email} was found!");
        }

        var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        _logger.LogInformation("Generated password reset token, Email: {UserEmail}", user.Email);

        NotificationService.SendPasswordResetEmailWithHangfire(user, passwordResetToken);
    }

    /// <exception cref="ObjectNotFoundException">If not user with the given email was found</exception>
    public async Task<(IdentityResult, ApplicationUser)> ResetPassword(string email, string passwordResetToken,
        string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email.Trim());

        if (user == null)
        {
            throw new ObjectNotFoundException($"No user with the email {email} was found!");
        }

        var result = await _userManager.ResetPasswordAsync(user, passwordResetToken, newPassword);

        if (result.Succeeded)
        {
            _logger.LogInformation("User reset their password, Email: {UserEmail}", user.Email);
            // Do we also want to sign the user in here? /Might/ be a security risk but it's convenient from a user perspective
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Code));
            _logger.LogInformation("User password reset failed, Email: {UserEmail}\n Error codes: {Errors}",
                user.Email, errors);
        }

        return (result, user);
    }

    public async Task<IdentityResult> DeleteUser(string userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.MarketplaceListings)
            .Include(u => u.MarketplacePurchases)
            .Include(u => u.TicketTransferRequests)
            .Include(u => u.AcceptedTicketTransfers)
            .Include(u => u.Orders)
            .Include(u => u.Tickets)
            .AsSplitQuery()
            .FirstOrThrowAsync(u => u.Id == userId);

        var activeListingsExists = user.MarketplaceListings?
            .Where(ml => ml.SoldAt != null || ml.Cancelled == false)
            .Any(ml => ml.SellerId == userId);

        if (activeListingsExists == true)
        {
            throw new TicketInMarketplaceListingException(
                "Cancel all marketplace listings before deleting the account");
        }

        var result = await _userManager.DeleteAsync(user);
        return result;
    }
}
