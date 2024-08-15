using Celebratix.Common.Models.DTOs.Business.Channel;
using FluentValidation;

namespace Celebratix.Common.Validators.Channel
{
    public class ChannelEventRequestValidator : AbstractValidator<ChannelEventRequest>
    {
        public ChannelEventRequestValidator()
        {
            RuleFor(m => m.Name)
                .NotEmpty()
                .NotNull()
                .MaximumLength(50);

            RuleFor(m => m.Slug)
                .MaximumLength(50);

            RuleFor(m => m.Slug)
                .MaximumLength(50);

            RuleFor(m => m.SelectedTicketTypes)
                .NotEmpty()
                .NotNull();
        }
    }
}
