using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Business.Events;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers.Admin;

[Area("admin")]
[Route("[area]/[controller]")]
[ApiController]
[AuthorizeRoles(Enums.Role.SuperAdmin)]
public class EventsController : ControllerBase
{
    private readonly EventService _eventService;

    public EventsController(EventService eventService, UserService userService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    public async Task<PagedResultDto<EventDetailedBusinessDto>> GetEvents(int page = 1, int pageSize = 8)
    {
        return await _eventService.GetEventsAsAdminDtos(page, pageSize);
    }
}
