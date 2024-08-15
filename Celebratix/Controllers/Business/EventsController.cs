using System.Security.Claims;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Business.Events;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers.Business;

[Area("business")]
[Route("[area]/[controller]")]
[ApiController]
[AuthorizeRoles(Enums.Role.Business, Enums.Role.TicketChecker)]
public class EventsController : ControllerBase
{
    private readonly EventService _eventService;
    private readonly UserService _userService;
    private readonly ChannelService _channelService;

    public EventsController(EventService eventService, UserService userService, ChannelService channelService)
    {
        _eventService = eventService;
        _userService = userService;
        _channelService = channelService;
    }

    [HttpGet]
    public async Task<PagedResultDto<EventBasicBusinessDto>> GetEvents(int page = 1, int pageSize = 8)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var businessId = await _userService.GetUserBusinessId(currentUserId);
        return await _eventService.GetEventsAsBusinessDtos(businessId, page, pageSize);
    }

    /// <response code="404">No event with given id was found</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventDetailedBusinessDto>> GetEvent(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasEventAccess(currentUserId, id);
        return await _eventService.GetEventAsBusinessDto(id);
    }

    [AllowAnonymous]
    [HttpGet("code/{code}")]
    public async Task<ActionResult<EventDetailedBusinessDto>> GetEventByCode(string code)
    {
        return await _eventService.GetEventAsBusinessDtoByCode(code);
    }

    [HttpPost("{id:int}/code")]
    public async Task<ActionResult<EventDetailedBusinessDto>> GenerateEventCode(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasEventAccess(currentUserId, id);
        return await _eventService.GenerateEventCode(id);
    }

    [HttpGet("channel/{channelId:Guid}")]
    public async Task<PagedResultDto<EventBasicBusinessDto>> GetEventsByChannel(Guid channelId, int page = 1, int pageSize = 8)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasChannelAccess(currentUserId, channelId);
        return await _eventService.GetEventsByChannelAsBusinessDtos(channelId, page, pageSize);
    }

    [HttpDelete("{id:int}/code")]
    public async Task<ActionResult<EventDetailedBusinessDto>> RemoveEventCode(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasEventAccess(currentUserId, id);
        return await _eventService.RemoveEventCode(id);
    }

    [HttpGet("{id:int}/tickets/stats")]
    public async Task<ActionResult<EventTicketStatsBusinessDto>> GetEventTicketStats(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasEventAccess(currentUserId, id);
        return await _eventService.GetEventTicketStatsAsBusinessDto(id);
    }

    /// <summary>
    /// Set the order of how the tickets should be displayed on the order page. Lowest index goes first.
    /// </summary>
    [HttpPut("{id:int}/tickets/arrange")]
    public async Task<ActionResult<EventDetailedBusinessDto>> ArrangeTickets(int id, EventArrangeTicketsBusinessDto[] dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasEventAccess(currentUserId, id);
        await _eventService.ArrangeTicketsDto(dto);
        return await _eventService.GetEventAsBusinessDto(id);
    }

    /// <response code="400">Slug is already used (slug_already_used)</response>
    /// <response code="400">Slug has invalid format (slug_invalid_format)</response>
    [HttpPost]
    [AuthorizeRoles(Enums.Role.Business)]
    public async Task<ActionResult<EventDetailedBusinessDto>> Create([FromForm] EventCreateBusinessDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await _eventService.CreateEvent(dto, currentUserId);
    }

    /// <response code="400">Slug is already used (slug_already_used)</response>
    /// <response code="400">Slug has invalid format (slug_invalid_format)</response>
    /// <response code="404">No company with given id was found</response>
    /// <response code="404">No valid user for the given contact id was found</response>
    [HttpPut("{id:int}")]
    [AuthorizeRoles(Enums.Role.Business)]
    public async Task<ActionResult<EventDetailedBusinessDto>> Edit(int id, [FromForm] EventUpdateBusinessDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasEventAccess(currentUserId, id);
        return await _eventService.UpdateEvent(id, dto);
    }

    [HttpPost("{id:int}/show")]
    [AuthorizeRoles(Enums.Role.Business)]
    public async Task<ActionResult<EventDetailedBusinessDto>> Show(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasEventAccess(currentUserId, id);
        return await _eventService.MakeEventVisible(id);
    }

    [HttpPost("{id:int}/hide")]
    [AuthorizeRoles(Enums.Role.Business)]
    public async Task<ActionResult<EventDetailedBusinessDto>> Hide(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasEventAccess(currentUserId, id);
        return await _eventService.HideEvent(id);
    }

    [HttpPost("{id:int}/ticket-type")]
    [AuthorizeRoles(Enums.Role.Business)]
    public async Task<ActionResult<EventTicketTypeBusinessDto>> Create(int id, EventTicketTypeUpdateBusinessDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasEventAccess(currentUserId, id);
        return await _eventService.CreateEventTicketType(id, dto);
    }

    /// <response code="404">No company with given id was found</response>
    /// <response code="404">No valid user for the given contact id was found</response>
    [HttpPut("{eventId:int}/ticket-type/{ticketTypeId:guid}")]
    [AuthorizeRoles(Enums.Role.Business)]
    public async Task<ActionResult<EventTicketTypeBusinessDto>> Edit(int eventId, Guid ticketTypeId, EventTicketTypeUpdateBusinessDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasEventAccess(currentUserId, eventId);
        return await _eventService.UpdateEventTicketType(ticketTypeId, dto);
    }
}
