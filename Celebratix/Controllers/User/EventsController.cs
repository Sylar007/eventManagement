using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.User.Events;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers.User;

[Area("user")]
[Route("[area]/[controller]")]
[ApiController]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly EventService _eventService;

    public EventsController(EventService eventService)
    {
        _eventService = eventService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<PagedResultDto<EventBasicDto>> GetEvents(int page = 1, int pageSize = 8, string? searchString = null, string? channel = null)
    {
        return await _eventService.GetEventsAsUserDtos(page, pageSize, searchString, channel);
    }

    /// <response code="404">No event with given id was found</response>
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventDetailedDto>> GetEvent(int id, string? code = null, string? affiliateCode = null)
    {
        return await _eventService.GetEventByIdAsUserDto(id, code, affiliateCode);
    }

    /// <response code="404">No event with given slug was found</response>
    [AllowAnonymous]
    [HttpGet("slug/{customSlug}")]
    public async Task<ActionResult<EventDetailedDto>> GetEvent(string customSlug, string? code = null, string? affiliateCode = null)
    {
        return await _eventService.GetEventBySlugAsUserDto(customSlug, code, affiliateCode);
    }
}
