using Celebratix.Common.Models.DTOs.Business.Channel;
using FluentValidation;

namespace Celebratix.Common.Validators.Channel;

public sealed class ChannelEventUpdateChannelResaleRequestValidator :
    AbstractValidator<ChannelUpdateResaleRequest>
{
    public ChannelEventUpdateChannelResaleRequestValidator()
    {
        RuleFor(request => request.DisabledDescription)
            .NotEmpty()
            .When(request => !request.Enabled)
            .WithMessage(channel => $"Channel with id '{channel.ChannelId}' is set to disabled but is missing the required 'Description'.");

        RuleFor(request => request.RedirectUrl)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(request => !string.IsNullOrEmpty(request.RedirectUrl))
            .WithMessage(channel =>
                $"Channel with id '{channel.ChannelId}' has a 'Redirect Url' which is not conform the 'http://host/page' format.");
    }
}