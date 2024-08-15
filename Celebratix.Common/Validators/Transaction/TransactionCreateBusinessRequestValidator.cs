using Celebratix.Common.Models.DTOs.Business;
using FluentValidation;

namespace Celebratix.Common.Validators.Transaction
{
    public class TransactionCreateBusinessRequestValidator : AbstractValidator<TransactionCreateBusinessRequest>
    {
        public TransactionCreateBusinessRequestValidator()
        {
            RuleFor(m => m.ChannelId)
                .NotEmpty()
                .NotNull();

            RuleFor(m => m.EventId)
                .NotEmpty()
                .NotNull();

            RuleFor(m => m.TrackingId)
                .NotEmpty()
                .NotNull();

            RuleFor(m => m.TransactionDate)
                .NotEmpty()
                .NotNull();

            RuleFor(m => m.FullName)
                .NotEmpty()
                .NotNull()
                .Length(1, 50);

            RuleFor(s => s.Email)
                .NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("A valid email is required")
                .Length(1, 100);
        }
    }
}
