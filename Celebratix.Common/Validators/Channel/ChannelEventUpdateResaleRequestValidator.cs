using Celebratix.Common.Models.DTOs.Business.Channel;
using FluentValidation;

namespace Celebratix.Common.Validators.Channel;

public class ChannelEventUpdateResaleRequestValidator :
    AbstractValidator<ChannelEventUpdateResaleRequest>
{
    public ChannelEventUpdateResaleRequestValidator()
    {
        RuleFor(request => request.ChannelUpdateResaleRequests.Count)
            .GreaterThan(0)
            .OverridePropertyName(nameof(ChannelEventUpdateResaleRequest.ChannelUpdateResaleRequests))
            .WithMessage("There should at least be 1 channel for which you want to update the resale options.");

        RuleFor(request => request.ChannelUpdateResaleRequests
                .GroupBy(channels => channels.ChannelId)
                .Count(group => group.Count() > 1))
            .Equal(0)
            .OverridePropertyName(nameof(ChannelEventUpdateResaleRequest.ChannelUpdateResaleRequests))
            .WithMessage("Duplicate channels are not allowed.");

        RuleForEach(request => request.ChannelUpdateResaleRequests)
            .SetValidator(new ChannelEventUpdateChannelResaleRequestValidator());
    }
}