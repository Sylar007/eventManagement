using Asp.Versioning;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Business.Channel;
using Celebratix.Common.Models.DTOs.Business.Events;
using Celebratix.Common.Services;
using Celebratix.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Celebratix.Controllers.Business.V2;

[ApiVersion(2.0)]
[Area("business")]
[Route("v{version:apiVersion}/[area]/[controller]")]
[ApiController]
[AuthorizeRoles(Enums.Role.Business, Enums.Role.TicketChecker, Enums.Role.SuperAdmin)]
public class EventsController : CelebratixControllerBase
{
    private readonly ChannelService _channelService;
    private readonly EventService _eventService;
    private readonly UserService _userService;

    public EventsController(EventService eventService, UserService userService, ChannelService channelService)
    {
        _eventService = eventService;
        _userService = userService;
        _channelService = channelService;
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(OperationResult<PagedResultDto<EventBasicBusinessDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<PagedResultDto<EventBasicBusinessDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetEvents([FromBody] EventSearchRequest? searchRequest)
    {
        var businessId = await _userService.GetUserBusinessIdByClaimsPrincipal(User);
        if (businessId.IsFailed)
        {
            return GetOkResult(businessId);
        }
        var result = await _eventService.GetEventsBySearch(businessId.Value, searchRequest);

        return GetOkResult(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OperationResult<EventBasicBusinessDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<EventBasicBusinessDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetEventById(int id)
    {
        var result = await _eventService.GetEventDto(id);

        return GetOkResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OperationResult<int>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(OperationResult<int>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEvent([FromForm] EventCreateRequest createRequest)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _eventService.CreateEventRequest(createRequest, currentUserId);

        return GetCreatedResult(result);
    }

    [HttpPut]
    [ProducesResponseType(typeof(OperationResult<EventBasicBusinessDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<EventBasicBusinessDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateEvent([FromForm] EventUpdateRequest updateRequest)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasEventAccess(currentUserId, updateRequest.Id);
        var result = await _eventService.UpdateEventRequest(updateRequest);

        return GetOkResult(result);
    }

    [HttpPut("{id:int}/{publish:bool}")]
    [ProducesResponseType(typeof(OperationResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PublishEvent(int id, bool publish)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasEventAccess(currentUserId, id);
        var result = await _eventService.PublishEvent(id, publish);

        return GetOkResult(result);
    }

    [HttpGet("{id:int}/ticket-types")]
    [ProducesResponseType(typeof(OperationResult<List<TicketTypeDashboardDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<List<TicketTypeDashboardDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetEventTicketTypes(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _eventService.GetEventTicketTypes(id);

        return GetOkResult(result);
    }

    [HttpPost("{id:int}/channel")]
    [ProducesResponseType(typeof(OperationResult<ChannelBusinessDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<ChannelBusinessDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEventChannel(int id, [FromForm] ChannelEventRequest request)
    {
        var businessId = await _userService.GetUserBusinessIdByClaimsPrincipal(User);
        if (businessId.IsFailed)
        {
            return GetOkResult(businessId);
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var accessValidation = await _userService.VerifyUserHasEventAccessV2(userId, id);
        if (accessValidation.IsFailed)
        {
            return GetOkResult<ChannelBusinessDto>(accessValidation);
        }
        var result = await _channelService.CreateChannelEvent(id, businessId.Value, request);

        return GetOkResult(result);
    }

    [HttpGet("{id:int}/channels")]
    [ProducesResponseType(typeof(OperationResult<List<ChannelBusinessDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<List<ChannelBusinessDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetChannelsForEvent(int id)
    {
        var businessId = await _userService.GetUserBusinessIdByClaimsPrincipal(User);
        if (businessId.IsFailed)
        {
            return GetOkResult(businessId);
        }
        var result = await _channelService.GetChannelsForEvent(id, businessId.Value);

        return GetOkResult(result);
    }

    [HttpPost("{eventId:int}/resale")]
    [ProducesResponseType(typeof(OperationResult<List<ChannelBusinessDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<List<ChannelBusinessDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateChannelResale(int eventId,
        [FromBody] ChannelEventUpdateResaleRequest request,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasEventAccess(currentUserId, eventId);

        var result = await _eventService
            .UpdateChannelResaleOptions(eventId, request, cancellationToken);

        return GetOkResult(result);
    }

    [HttpPost("{eventId:int}/publish")]
    [ProducesResponseType(typeof(OperationResult<List<ChannelBusinessDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<List<ChannelBusinessDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Publish(int eventId, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasEventAccess(currentUserId, eventId);

        var result = await _eventService
            .Publish(eventId, cancellationToken);

        return GetOkResult(result);
    }
}
