using Celebratix.Common.Models.DTOs.Business.Channel;
using FluentValidation;

namespace Celebratix.Common.Validators.Channel
{
    public class ChannelRequestValidator : AbstractValidator<ChannelRequest>
    {
        public ChannelRequestValidator()
        {
            RuleFor(m => m.Name)
                .NotEmpty()
                .NotNull()
                .Length(1, 50);

            RuleFor(m => m.Slug)
                .MaximumLength(50);

            RuleFor(m => m.Color)
                .NotEmpty()
                .MaximumLength(9)
                .Matches(@"^#(?:[0-9a-fA-F]{3}){1,2}$")
                .WithMessage("Invalid color code.");
        }
    }
}
