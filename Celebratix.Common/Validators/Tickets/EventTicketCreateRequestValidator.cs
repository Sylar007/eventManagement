using FluentValidation;
using Celebratix.Common.Models.DTOs.Business.Events;

namespace Celebratix.Common.Validators.Tickets
{
    public class EventTicketCreateRequestValidator : AbstractValidator<EventTicketCreateRequest>
    {
        public EventTicketCreateRequestValidator()
        {
            RuleFor(p => p.EventId)
                .NotNull()
                .GreaterThan(0);

            RuleFor(p => p.Name)
                .NotNull()
                .NotEmpty();

            RuleFor(p => p.CategoryId)
                .NotNull();

            RuleFor(p => p.Price)
                .NotNull();

            RuleFor(p => p.ServiceFee)
                .NotNull();

            RuleFor(p => p.MinTicketsPerPurchase)
                .NotNull()
                .GreaterThan(0)
                .LessThanOrEqualTo(p => p.MaxTicketsPerPurchase);

            RuleFor(p => p.MaxTicketsPerPurchase)
                .NotNull()
                .GreaterThan(0)
                .GreaterThanOrEqualTo(p => p.MinTicketsPerPurchase);

            RuleFor(p => p.AvailableFrom)
                .NotNull()
                .GreaterThan(DateTimeOffset.Now)
                .LessThanOrEqualTo(p => p.AvailableUntil);

            RuleFor(p => p.AvailableUntil)
                .NotNull()
                .GreaterThanOrEqualTo(DateTimeOffset.Now)
                .GreaterThanOrEqualTo(p => p.AvailableFrom);
        }
    }
}