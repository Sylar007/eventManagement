using Celebratix.Common.Models.DTOs.Business.Events;
using FluentValidation;

namespace Celebratix.Common.Validators.Events
{
    public class EventCreateRequestValidator : AbstractValidator<EventCreateRequest>
    {
        public EventCreateRequestValidator()
        {
            RuleFor(m => m.Name)
                .NotEmpty()
                .NotNull()
                .Length(1, 50);

            RuleFor(m => m.Description)
                .MaximumLength(500);

            RuleFor(m => m.StartDate)
                .NotNull().NotNull()
                .GreaterThan(DateTimeOffset.Now);

            RuleFor(m => m.EndDate)
                .NotNull()
                .GreaterThan(m => m.StartDate);

            RuleFor(m => m.Location)
                .NotNull()
                .MaximumLength(100);

            RuleFor(m => m.City)
                .MaximumLength(50);

            RuleFor(m => m.AddressLine1)
                .MaximumLength(200);

            RuleFor(m => m.AddressLine2)
                .MaximumLength(200);

            RuleFor(m => m.Postcode)
                .MaximumLength(20);

            RuleFor(m => m.Website)
                .MaximumLength(100);

            RuleFor(m => m.CurrencyId)
                .NotNull();
        }
    }
}
