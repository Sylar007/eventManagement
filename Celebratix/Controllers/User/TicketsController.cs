using System.Security.Claims;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.User.Events;
using Celebratix.Common.Models.DTOs.User.Tickets;
using Celebratix.Common.Models.DTOs.User.TicketTransfer;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers.User;

[Area("user")]
[Route("[area]/[controller]")]
[ApiController]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly TicketService _ticketService;
    private readonly TicketTransferService _ticketTransferService;

    public TicketsController(TicketService ticketService, TicketTransferService ticketTransferService)
    {
        _ticketService = ticketService;
        _ticketTransferService = ticketTransferService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TicketQrDto>> GetTicket(Guid id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await _ticketService.GetTicketForOwner(currentUserId, id);
    }

    [HttpGet("{id:guid}/details")]
    public async Task<ActionResult<TicketDto>> GetTicketDetails(Guid id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await _ticketService.GetTicketDetailsForOwner(currentUserId, id);
    }

    /// <summary>
    /// Only gets available tickets. I.e. tickets in active transfer offers or marketplace listings are not included
    /// </summary>
    [HttpGet("upcoming")]
    public async Task<PagedResultDto<TicketDto>> GetAllForUpcomingEvents(int page = 1, int pageSize = 20)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var tickets = await _ticketService.GetAllUsersTicketsForUpcomingEvents(currentUserId, page, pageSize);
        return tickets;
    }

    /// <summary>
    /// Gets all tickets, excluding those that cannot be scanned
    /// </summary>
    [HttpGet("order/{orderId:guid}")]
    public async Task<ActionResult<List<TicketQrDto>>> GetTicketsForOrder(Guid orderId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var tickets = await _ticketService.GetUsersTicketsForOrder(currentUserId, orderId, true);
        return tickets;
    }

    /// <summary>
    /// Gets all tickets, including those that cannot be scanned
    /// </summary>
    [HttpGet("order/{orderId:guid}/unfiltered")]
    public async Task<ActionResult<List<TicketQrDto>>> GetTicketsForOrderV2(Guid orderId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var tickets = await _ticketService.GetUsersTicketsForOrder(currentUserId, orderId, false);
        return tickets;
    }

    [HttpGet("transfers/accepted")]
    public async Task<ActionResult<PagedResultDto<TicketTransferOfferDto>>> GetAcceptedTransferOffers(int page = 1, int pageSize = 20)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var transferOffers = await _ticketTransferService.GetAcceptedTransferOffers(currentUserId, page, pageSize);
        return transferOffers;
    }

    [HttpGet("transfers/outgoing")]
    public async Task<ActionResult<PagedResultDto<TicketTransferOfferDto>>> GetOutgoingTransferOffers(bool onlyActive, int page = 1, int pageSize = 20)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var transferOffers = await _ticketTransferService.GetOutgoingTransferOffers(currentUserId, onlyActive, page, pageSize);
        return transferOffers;
    }

    /// <summary>
    /// Currently only permitted for the Transferor, when there is a need for another user to
    /// access this endpoint, then the permission (and/or the query) will be adjusted.
    /// </summary>
    [HttpGet("transfers/{id:guid}")]
    public async Task<ActionResult<TicketTransferOfferDto>> GetTransferOfferById(Guid id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await _ticketTransferService.GetTransferOfferByIdForUser(currentUserId, id);
    }

    [HttpGet("transfers/code/{code}")]
    public async Task<ActionResult<TicketTransferOfferDto>> GetTransferOfferByCode(string code)
    {
        return await _ticketTransferService.GetTransferOfferByCode(code);
    }

    /// <response code="400">Ticket has already been scanned/checked in (ticket_already_scanned)</response>
    /// <response code="400">The ticket is in an active marketplace listing (ticket_in_marketplace_listing)</response>
    /// <response code="400">The ticket is in an active transfer offer (ticket_in_transfer_offer)</response>
    /// <response code="400">The ticket was sold or transferred (ticket_sold_or_transferred)</response>
    /// <response code="400">The event end time has passed (event_end_time_passed)</response>
    [HttpPost("transfers")]
    public async Task<ActionResult<TicketTransferOfferDto>> CreateTransferOffer(Guid ticketId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await _ticketTransferService.CreateTransferOffer(currentUserId, ticketId);
    }

    /// <response code="400">Transfer offer is not available to accept or the user is the same as the transferor (transfer_offer_not_available)</response>
    [HttpPost("transfers/accept")]
    public async Task<ActionResult<TicketTransferOfferDto>> AcceptTransferOffer([FromBody] string code)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await _ticketTransferService.AcceptTransferOffer(currentUserId, code);
    }

    /// <response code="400">Transfer offer is not available to cancel (transfer_offer_not_available)</response>
    [HttpPost("transfers/{id:guid}/cancel")]
    public async Task<ActionResult<TicketTransferOfferDto>> CancelTransferOffer(Guid id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await _ticketTransferService.CancelTransferOffer(currentUserId, id, TicketTransferService.CancelReason.UserAction);
    }

    /// <summary>
    /// Returns all the events for which the user owns at least 1 ticket
    /// </summary>
    [HttpGet("events")]
    public async Task<ActionResult<PagedResultDto<EventWithTicketDataDto>>> GetEventsByOwnedTickets([FromQuery]
            Enums.EventStatus? eventStatus = null, int page = 1, int pageSize = 20)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await _ticketService.GetEventsByOwnedTickets(currentUserId, eventStatus, page, pageSize);
    }

    /// <summary>
    /// Returns all the tickets the user owns for a specific event
    /// </summary>
    [HttpGet("event/{eventId:int}")]
    public async Task<ActionResult<List<TicketQrDto>>> GetOwnedTicketsForEvent(int eventId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await _ticketService.GetOwnedTicketsForEvent(currentUserId, eventId);
    }

    /// <summary>
    /// Returns all the tickets the user owns of a specific ticketType
    /// </summary>
    [HttpGet("ticket-type/{ticketTypeId:guid}")]
    public async Task<ActionResult<List<TicketQrDto>>> GetOwnedTicketsForTicketType(Guid ticketTypeId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await _ticketService.GetOwnedTicketsForTicketType(currentUserId, ticketTypeId);
    }
}
