using Celebratix.Common.Configs;
using Celebratix.Common.Models.DbModels;
using Hangfire;

namespace Celebratix.Common.Services;

// In general this class contains a lot of duplicate code.
// Instead of separate methods the template id could be passed in for some (but not for others)
// However as there aren't that many different emails it's best probably to have a separate method for each
// to keeping the complexity down.
public partial class NotificationService
{
    /// <summary>
    /// Queues the email task with Hangfire and returns the hangfire work id
    /// </summary>
    public static string SendPasswordResetEmailWithHangfire(ApplicationUser user, string passwordResetToken)
    {
        if (user.Email == null) throw new ArgumentException("User requires email");

        return BackgroundJob.Enqueue<Celebratix.Common.Services.NotificationService>(x =>
            x.SendPasswordResetEmail(user.FullName, user.Email, passwordResetToken)
        );
    }

    public async Task SendPasswordResetEmail(string name, string to, string passwordResetToken)
    {
        // Query string needs to be encoded to allow for more than one value to be sent via Firebase dynamic link.
        var passwordResetPathWithValues = _urlConfig.PasswordResetRelativeRef
            .Replace(UrlConfig.TokenIdentifier, passwordResetToken).Replace(UrlConfig.EmailIdentifier, to);

        var passwordResetLink = _urlConfig.BusinessBaseUrl + passwordResetPathWithValues;

        await CreateAndSendSendGridEmailAsync(
            to,
            _sendGridConfig.ForgotPasswordId,
            new
            {
                Name = name,
                Link = passwordResetLink
            });
    }

    /// <summary>
    /// Queues the email task with Hangfire and returns the hangfire work id
    /// </summary>
    public static string SendEmailConfirmationWithHangfire(ApplicationUser user, string token)
    {
        if (user.Email == null) throw new ArgumentException("User requires email");

        return BackgroundJob.Enqueue<NotificationService>(x =>
            x.SendEmailConfirmation(user.FullName, user.Email, token)
        );
    }

    public async Task SendEmailConfirmation(string name, string to, string token)
    {
        // Query string needs to be encoded to allow for more than one value to be sent via Firebase dynamic link.
        var emailConfirmationPathWithValues = _urlConfig.EmailConfirmationRelativeRef
            .Replace(UrlConfig.TokenIdentifier, token).Replace(UrlConfig.EmailIdentifier, to);

        var passwordResetLink = _urlConfig.AppBaseUrl + emailConfirmationPathWithValues;

        await CreateAndSendSendGridEmailAsync(
            to,
            _sendGridConfig.EmailConfirmationId,
            new
            {
                Name = name,
                Link = passwordResetLink
            });
    }

    /// <summary>
    /// Queues the email task with Hangfire and returns the hangfire work id
    /// </summary>
    public static string SendAdminAccountSetupEmailWithHangfire(ApplicationUser user, string token)
    {
        if (user.Email == null) throw new ArgumentException("User requires email");

        return BackgroundJob.Enqueue<NotificationService>(x =>
            x.SendAdminAccountSetupEmail(user.FullName, user.Email, token)
        );
    }

    public async Task SendAdminAccountSetupEmail(string name, string to, string token)
    {
        // Query string needs to be encoded to allow for more than one value to be sent via Firebase dynamic link.
        var activateAccountPathWithValues = _urlConfig.ActivateAccountRelativeRef
            .Replace(UrlConfig.TokenIdentifier, token).Replace(UrlConfig.EmailIdentifier, to);

        var link = _urlConfig.AdminBaseUrl + activateAccountPathWithValues;

        await CreateAndSendSendGridEmailAsync(
            to,
            _sendGridConfig.AdminAccountSetupId,
            new
            {
                Name = name,
                Link = link
            });
    }

    /// <summary>
    /// Queues the email task with Hangfire and returns the hangfire work id
    /// </summary>
    public static string SendCompanyAccountSetupEmailWithHangfire(ApplicationUser user, Business business, string passwordResetToken)
    {
        if (user.Email == null) throw new ArgumentException("User requires email");

        return BackgroundJob.Enqueue<Celebratix.Common.Services.NotificationService>(x =>
            x.SendBusinessAccountSetupEmail(user.FullName, user.Email, business.Name, passwordResetToken)
        );
    }

    public async Task SendBusinessAccountSetupEmail(string name, string to, string companyName, string token)
    {
        // Query string needs to be encoded to allow for more than one value to be sent via Firebase dynamic link.
        var activateAccountPathWithValues = _urlConfig.ActivateAccountRelativeRef
            .Replace(UrlConfig.TokenIdentifier, token).Replace(UrlConfig.EmailIdentifier, to);

        var link = _urlConfig.BusinessBaseUrl + activateAccountPathWithValues;

        await CreateAndSendSendGridEmailAsync(
            to,
            _sendGridConfig.BusinessAccountSetupId,
            new
            {
                Name = name,
                CompanyName = companyName,
                Link = link
            });
    }

    /// <summary>
    /// Queues the email task with Hangfire and returns the hangfire work id
    /// </summary>
    public static string SendTicketPurchaseCompleteEmailWithHangfire(ApplicationUser user, Order order)
    {
        if (user.Email == null) throw new ArgumentException("User requires email");

        return BackgroundJob.Enqueue<NotificationService>(x =>
            x.SendTicketPurchaseCompleteEmail(user.FirstName, user.Email, order.Id)
        );
    }

    public async Task SendTicketPurchaseCompleteEmail(string? firstName, string to, Guid orderId)
    {
        // Query string needs to be encoded to allow for more than one value to be sent via Firebase dynamic link.
        var magic = _magicService.CreateMagicForOrder(orderId);
        var magicPathWithValues = _urlConfig.MagicOrderRelativeRef
            .Replace(UrlConfig.MagicIdentifier, magic);

        var magicLink = _urlConfig.AppBaseUrl + magicPathWithValues;

        await CreateAndSendSendGridEmailAsync(
            to,
            _sendGridConfig.TicketPurchaseCompleteId,
            new
            {
                FirstName = firstName,
                OrderLink = magicLink,
            });
    }

    /// <summary>
    /// Queues the email task with Hangfire and returns the hangfire work id
    /// </summary>
    public static string SendCustomEventEmailWithHangfire(ApplicationUser user, string customEmailTemplateId)
    {
        if (user.Email == null) throw new ArgumentException("User requires email");

        return BackgroundJob.Enqueue<NotificationService>(x =>
            x.SendCustomEventEmail(user.FirstName, user.Email, customEmailTemplateId)
        );
    }

    public async Task SendCustomEventEmail(string? firstName, string to, string templateId)
    {
        await CreateAndSendSendGridEmailAsync(
            to,
            templateId,
            new
            {
                FirstName = firstName
            });
    }
}
