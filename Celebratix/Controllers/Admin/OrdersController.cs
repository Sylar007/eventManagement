using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Admin.Orders;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers.Admin;

[Area("admin")]
[Route("[area]/events/{eventId:int}/[controller]")]
[ApiController]
[AuthorizeRoles(Enums.Role.SuperAdmin)]
public class OrdersController : ControllerBase
{
    private readonly OrderAggregatorService _orderAggregatorService;

    public OrdersController(OrderAggregatorService orderAggregatorService)
    {
        _orderAggregatorService = orderAggregatorService;
    }

    [HttpGet]
    public async Task<PagedResultDto<OrderBasicAdminDto>> GetOrders(int eventId, int page = 1, int pageSize = 8)
    {
        return await _orderAggregatorService.GetOrdersAsAdminDtos(eventId, page, pageSize);
    }
}
