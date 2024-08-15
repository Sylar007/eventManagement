using Celebratix.Common.Configs;
using Celebratix.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Celebratix.Common.Services;

public partial class NotificationService
{
    private readonly CelebratixDbContext _dbContext;
    private readonly ISendGridClient _sendGridClient;
    private readonly SendGridConfig _sendGridConfig;
    private readonly UrlConfig _urlConfig;
    private readonly MagicService _magicService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(CelebratixDbContext dbContext, ISendGridClient sendGridClient,
        IOptions<SendGridConfig> sendGridConfig, IOptions<UrlConfig> urlConfig, MagicService magicService, ILogger<NotificationService> logger)
    {
        _dbContext = dbContext;
        _sendGridClient = sendGridClient;
        _sendGridConfig = sendGridConfig.Value;
        _urlConfig = urlConfig.Value;
        _logger = logger;
        _magicService = magicService;
    }

    private SendGridMessage CreateSingleTemplateSendGridEmail(string to, string templateId, object dynamicData)
    {
        return MailHelper.CreateSingleTemplateEmail(
            new EmailAddress(_sendGridConfig.SendFrom, _sendGridConfig.SenderName),
            new EmailAddress(to),
            templateId,
            dynamicData);
    }


    /// <summary>
    /// Returns success status & logs with warning if mail failed to be sent
    /// </summary>
    private async Task CreateAndSendSendGridEmailAsync(string to, string templateId, object dynamicData)
    {
        var message = CreateSingleTemplateSendGridEmail(to, templateId, dynamicData);
        await SendSendGridEmailAsync(message);
    }

    /// <summary>
    /// Returns success status & logs with warning if mail failed to be sent
    /// </summary>
    private async Task SendSendGridEmailAsync(SendGridMessage message)
    {
        var recipients = string.Join("; ", message.Personalizations.SelectMany(p => p.Tos.Select(tos => tos.Email)));
        var detailsString = $"Recipient(s): {recipients}\n" +
                            $"TemplateId  : {message.TemplateId}\n";

        var response = await _sendGridClient.SendEmailAsync(message);

        if (response.IsSuccessStatusCode)
            _logger.LogInformation($"Send message to sendgrid\n" + detailsString);
        else
        {
            var error = $"Status Code : {response.StatusCode}\n" +
                        $"Body        : {await response.Body.ReadAsStringAsync()}\n";
            _logger.LogWarning($"Failed to send message to sendgrid\n" + detailsString + error);
        }
    }
}
