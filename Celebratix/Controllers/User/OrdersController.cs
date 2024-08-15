using System.Security.Claims;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.User.Events;
using Celebratix.Common.Models.DTOs.User.Orders;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers.User;

[Area("user")]
[Route("[area]/[controller]")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly OrderAggregatorService _orderAggregatorService;
    private readonly MagicService _magicService;

    public OrdersController(OrderService orderService, OrderAggregatorService orderAggregatorService, MagicService magicService)
    {
        _orderService = orderService;
        _orderAggregatorService = orderAggregatorService;
        _magicService = magicService;
    }

    [HttpGet]
    public async Task<PagedResultDto<OrderBasicDto>> GetOrders([FromQuery] List<Enums.OrderStatus>? statusFilter = null,
        Enums.EventStatus? eventStatus = null, int? eventId = null, int page = 1, int pageSize = 20)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var orders = await _orderAggregatorService.GetOrdersForUserAsDtos(currentUserId, statusFilter, eventStatus, eventId, page, pageSize);
        return orders;
    }

    [HttpGet("{orderId:guid}")]
    public async Task<ActionResult<OrderDetailedDto>> GetOrder(Guid orderId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var order = await _orderAggregatorService.GetOrderForUserAsDto(currentUserId, orderId);
        return order;
    }

    /// <summary>
    /// Placing a new "primary market" order
    /// </summary>
    /// <response code="400">If the user tries to buy an unallowed amount of tickets in the order (allowed_tickets_limit_exceeded)</response>
    /// <response code="400">If the amount of tickets the user tried to buy aren't available (requested_tickets_not_available)</response>
    [HttpPost("primary")]
    public async Task<ActionResult<OrderCreateResponseDto>> PlacePrimaryMarketOrder(PrimaryMarketOrderCreateDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var orderResponseDto = await _orderService.PlacePrimaryMarketOrder(currentUserId, dto);
        return orderResponseDto;
    }

    /// <summary>
    /// Placing a new marketplace order
    /// </summary>
    /// <response code="400">If the marketplace listing isn't available anymore (listing_not_available)</response>
    [HttpPost("marketplace/{listingId:guid}")]
    public async Task<ActionResult<OrderCreateResponseDto>> PlaceMarketplaceOrder(Guid listingId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var orderResponseDto = await _orderService.PlaceMarketplaceOrder(currentUserId, listingId);
        return orderResponseDto;
    }

    /// <summary>
    /// Can only be done before payment information has been submitted
    /// </summary>
    /// <response code="404">No cancellable order was found. I.e. the order might exist but not be cancellable by the user</response>
    [HttpPost("{orderId:guid}/cancel")]
    public async Task<ActionResult> CancelOrder(Guid orderId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _orderService.CancelOrder(currentUserId, orderId);
        return Ok();
    }

    /// <summary>
    /// Get ticket and event data from magic, the tickets field will only be populated if the order is completed
    /// Doesn't need authentication
    /// Excludes tickets that cannot be scanned
    /// </summary>
    [HttpGet("magic/{magic}")]
    [AllowAnonymous]
    public async Task<ActionResult<OrderFromMagicDto>> GetOrderFromMagic(string magic)
    {
        return await _magicService.GetOrderFromMagicAsDto(magic, true);
    }

    // TODO: move this to the v2 controller
    /// <summary>
    /// Get ticket and event data from magic, the tickets field will only be populated if the order is completed
    /// Doesn't need authentication
    /// Includes tickets that cannot be scanned
    /// </summary>
    [HttpGet("magic/{magic}/unfiltered")]
    [AllowAnonymous]
    public async Task<ActionResult<OrderFromMagicDto>> GetOrderFromMagicV2(string magic)
    {
        return await _magicService.GetOrderFromMagicAsDto(magic, false);
    }

    /// <summary>
    /// Get all events the user has orders for, can be filtered
    /// </summary>
    [HttpGet("events")]
    public async Task<ActionResult<PagedResultDto<EventBasicDto>>> GetEventsByUserOrders([FromQuery] List<Enums.OrderStatus>? statusFilter = null, Enums.EventStatus? eventStatus = null, int page = 1, int pageSize = 20)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await _orderAggregatorService.GetOrdersForUserByEventAsDtos(currentUserId, statusFilter, eventStatus, page, pageSize);
    }
}
