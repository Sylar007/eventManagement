using System.Security.Claims;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Business.Orders;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers.Business;

[Area("business")]
[Route("[area]/events/{eventId:int}/[controller]")]
[ApiController]
[AuthorizeRoles(Enums.Role.Business)]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly OrderAggregatorService _orderAggregatorService;
    private readonly UserService _userService;

    public OrdersController(OrderService orderService, OrderAggregatorService orderAggregatorService, UserService userService)
    {
        _orderService = orderService;
        _orderAggregatorService = orderAggregatorService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<PagedResultDto<OrderBasicBusinessDto>> GetOrders(int eventId, int page = 1, int pageSize = 8)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasEventAccess(currentUserId.ToString(), eventId);
        return await _orderAggregatorService.GetOrdersAsBusinessDtos(eventId, page, pageSize);
    }

    /// <response code="404">No order with given id was found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDetailedBusinessDto>> GetOrder(Guid id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasOrderAccess(currentUserId, id);
        return await _orderAggregatorService.GetOrderAsBusinessDto(id);
    }
}
