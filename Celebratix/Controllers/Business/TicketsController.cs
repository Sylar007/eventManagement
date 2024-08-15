using System.Security.Claims;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Business.Tickets;
using Celebratix.Common.Services;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers.Business;

[Area("business")]
[Route("[area]/[controller]")]
[ApiController]
[AuthorizeRoles(Enums.Role.Business, Enums.Role.TicketChecker)]
public class TicketsController : CelebratixControllerBase
{
    private readonly TicketService _ticketService;
    private readonly UserService _userService;

    public TicketsController(TicketService ticketService, UserService userService)
    {
        _ticketService = ticketService;
        _userService = userService;
    }

    /// <response code="404">No ticket with the given id was found</response>
    /// <response code="400">The supplied time for the ticket is outside the allowed range (ticket_qr_time_expired)</response>
    /// <response code="400">Ticket has already been scanned/checked in (ticket_already_scanned)</response>
    /// <response code="400">Ticket is not one of the allowed types (bad_ticket_type_on_ticket)</response>
    /// <response code="400">The ticket isn't in the supplied event (bad_event_on_ticket)</response>
    /// <response code="400">The ticket is in an active marketplace listing (ticket_in_marketplace_listing)</response>
    /// <response code="400">The event end time has passed (event_end_time_passed)</response>
    [HttpPost("event/{eventId:int}/scan")]
    public async Task<ActionResult<TicketScanningResponseDto>> ScanTicket(int eventId, TicketScanningInputDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasEventAccess(currentUserId, eventId);
        return await _ticketService.ScanTicket(eventId, dto);
    }

    /// <response code="404">No ticket with the given id was found</response>
    /// <response code="400">The supplied time for the ticket is outside the allowed range (ticket_qr_time_expired)</response>
    /// <response code="400">Ticket has already been scanned/checked in (ticket_already_scanned)</response>
    /// <response code="400">Ticket is not one of the allowed types (bad_ticket_type_on_ticket)</response>
    /// <response code="400">The ticket isn't in the supplied event (bad_event_on_ticket)</response>
    /// <response code="400">The ticket is in an active marketplace listing (ticket_in_marketplace_listing)</response>
    /// <response code="400">The ticket is in an active transfer offer (ticket_in_transfer_offer)</response>
    /// <response code="400">The ticket is unavailable to refund (ticket_is_unavailable_to_refund)</response>
    [HttpPost("event/{eventId:int}/refund")]
    public async Task<ActionResult<TicketScanningResponseDto>> RefundTicket(int eventId, TicketScanningInputDto scanningRequest)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasEventAccess(currentUserId, eventId);
        return await _ticketService.RefundTicket(eventId, scanningRequest);
    }

    [HttpPut("event/{eventId:int}/status)")]
    [ProducesResponseType(typeof(Ticket), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateRefundTicket(int eventId, TicketUpdateRefundInputDto ticketUpdateRefundInputDto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasEventAccess(currentUserId, eventId);
        var result = await _ticketService.UpdateRefundTicket(ticketUpdateRefundInputDto);

        return GetOkResult(result);
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(PagedResultDto<TicketResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTicketsBySearch(TicketSearchRefundInputDto searcInputDto, int page = 1, int pageSize = 10)
    {
        var businessId = await _userService.GetUserBusinessIdByClaimsPrincipal(User);
        if (businessId.IsFailed)
        {
            return GetOkResult(businessId);
        }
        var result = await _ticketService.GetTicketsBySearch(searcInputDto, businessId.Value, page, pageSize);

        return GetOkResult(result);
    }
}
