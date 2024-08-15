using Celebratix.Common.Models.DTOs.Business;
using FluentValidation;

namespace Celebratix.Common.Validators.Tracking
{
    public class BusinessTrackerCreateRequestValidator : AbstractValidator<BusinessTrackerCreateRequest>
    {
        public BusinessTrackerCreateRequestValidator()
        {
            RuleFor(m => m.ChannelId)
                .NotEmpty()
                .NotNull();

            RuleFor(m => m.AffiliateId)
                .NotEmpty()
                .NotNull();

            RuleFor(m => m.Name)
                .NotEmpty()
                .NotNull()
                .Length(1, 50);
        }
    }
}
