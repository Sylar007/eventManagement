using System.Data;
using Celebratix.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Celebratix.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[ApiController]
public class ErrorController : ControllerBase
{
    [Route("error")]
    public ActionResult<ProblemDetails> Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        var exception = context?.Error;
        var path = context?.Path;
        if (exception == null) return StatusCode(500);

        var problemDetails = new ProblemDetails
        {
            Instance = path
        };

        var code = 500; // Internal Server Error by default
        string? type = null;
        string? title = null;

        switch (exception)
        {
            case ForbiddenException e:
                code = 403; //Forbidden
                title = exception.Message;
                break;
            case ObjectNotFoundException e:
                code = 404; // Not Found
                title = exception.Message;
                break;
            case InvalidOperationException e:
                type = "invalid_operation";
                code = 403; // Forbidden
                title = exception.Message;
                break;
            case ConstraintException e:
                type = "constraint_violation";
                code = 403; // Forbidden
                title = exception.Message;
                break;
            case ArgumentOutOfRangeException e:
                title = "argument_out_of_range";
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case ArgumentException e:
                type = "illegal_argument";
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case DuplicateNameException e:
                type = "duplicate_name";
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case ObjectAlreadyExistsException e:
                type = ObjectAlreadyExistsException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case DbUpdateConcurrencyException e:
                title = "database_concurrency_error";
                code = 500;
                break;
            case InvalidPasswordException e:
                type = InvalidPasswordException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case InvalidRefreshTokenException e:
                type = InvalidRefreshTokenException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case EmailAlreadyInUseException e:
                type = EmailAlreadyInUseException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case UnsupportedRoleException e:
                type = UnsupportedRoleException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case AccountAlreadyActivatedException e:
                type = AccountAlreadyActivatedException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case NotMemberOfBusinessException e:
                type = NotMemberOfBusinessException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case EmailNotConfirmedException e:
                type = EmailNotConfirmedException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case PhoneNotConfirmedException e:
                type = PhoneNotConfirmedException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case SmsSendingFailedException e:
                type = SmsSendingFailedException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case EmailAlreadyConfirmedException e:
                type = EmailAlreadyConfirmedException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case AllowedTicketsLimitExceededException e:
                type = AllowedTicketsLimitExceededException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case RequestedTicketsNotAvailableException e:
                type = RequestedTicketsNotAvailableException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case ListingNotAvailableException e:
                type = ListingNotAvailableException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case BadOrderStatusException e:
                type = BadOrderStatusException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case EventEndTimePassedException e:
                type = EventEndTimePassedException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case PayoutsNotEnabledOnUserException e:
                type = PayoutsNotEnabledOnUserException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case TicketAlreadyScannedException e:
                type = TicketAlreadyScannedException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case TicketBadTicketTypeException e:
                type = TicketBadTicketTypeException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case TicketQrTimeExpiredException e:
                type = TicketQrTimeExpiredException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case TicketWrongEventException e:
                type = TicketWrongEventException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case TicketInMarketplaceListingException e:
                type = TicketInMarketplaceListingException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case TicketInTransferOfferException e:
                type = TicketInTransferOfferException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case TicketSoldOrTransferredException e:
                type = TicketSoldOrTransferredException.ProblemDetailType;
                code = 401; // Unauthorized
                title = exception.Message;
                break;
            case TransferOfferNotAvailableException e:
                type = TransferOfferNotAvailableException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case SlugAlreadyUsedException e:
                type = SlugAlreadyUsedException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case SlugInvalidFormatException e:
                type = SlugInvalidFormatException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case UserDeletedException e:
                type = UserDeletedException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case JwtTokenInvalidException e:
                type = JwtTokenInvalidException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case TicketForRefundWaitingReviewException e:
                type = TicketForRefundWaitingReviewException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case TicketForRefundApprovedException e:
                type = TicketForRefundApprovedException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case TicketForRefundDeniedException e:
                type = TicketForRefundDeniedException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case TicketUnavailableException e:
                type = TicketUnavailableException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case InvalidUrlException e:
                type = InvalidUrlException.ProblemDetailType;
                code = 400; // Bad Request
                title = exception.Message;
                break;
            case BadRequestException e:
                type = "bad_request";
                code = 400; // Bad Request
                title = exception.Message;
                break;
            /*
            case DbUpdateException e:
                title = "Database exception";
                code = 500; // Internal server error. Often these errors could be things like bad requests caused by "concurrency" issues
                break;
            case TransactionAbortedException e:
                title = "Transaction exception";
                code = 500; // Internal server error. Often these errors could be things like bad requests caused by "concurrency" issues
                break;
            */
            default:
                title = exception.Message;
                code = 500;
                break;
        }

        problemDetails.Type = type;
        problemDetails.Status = code;
        problemDetails.Title = title;
        return problemDetails;
    }
}
