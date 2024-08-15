using Celebratix.Common.Extensions;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs.Business.Tickets;
using FluentValidation;
using static Celebratix.Common.Models.Enums;

namespace Celebratix.Common.Validators.Tickets
{
    public class TicketUpdateRefundInputDtoValidator : AbstractValidator<TicketUpdateRefundInputDto>
    {
        public TicketUpdateRefundInputDtoValidator()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty()
               .WithMessage("Id cannot be empty or null.");
        }
    }
}