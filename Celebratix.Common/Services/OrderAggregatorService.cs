using Celebratix.Common.Extensions;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Admin.Orders;
using Celebratix.Common.Models.DTOs.Business.Orders;
using Celebratix.Common.Models.DTOs.User.Events;
using Celebratix.Common.Models.DTOs.User.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Celebratix.Common.Services;

public class OrderAggregatorService
{
    private readonly CelebratixDbContext _dbContext;
    private readonly MagicService _magicService;
    private readonly ILogger<OrderAggregatorService> _logger;

    public OrderAggregatorService(CelebratixDbContext dbContext, MagicService magicService, ILogger<OrderAggregatorService> logger)
    {
        _dbContext = dbContext;
        _magicService = magicService;
        _logger = logger;
    }

    public async Task<PagedResultDto<OrderBasicAdminDto>> GetOrdersAsAdminDtos(int eventId, int page, int pageSize)
    {
        var orders = await _dbContext.Orders
            .Where(o => o.TicketType!.EventId == eventId)
            .OrderByDescending(o => o.CreatedAt)
            .Include(o => o.TicketType)
                .ThenInclude(t => t!.Event)
            .Include(o => o.Currency)
            .Select(o => new OrderBasicAdminDto(o))
            .AsSplitQuery()
            .ToPagedResult(page, pageSize);

        return orders;
    }

    public async Task<PagedResultDto<OrderBasicBusinessDto>> GetOrdersAsBusinessDtos(int eventId, int page, int pageSize)
    {
        var orders = await _dbContext.Orders
            .Where(o => o.TicketType!.EventId == eventId)
            .Where(o => o.Status == Enums.OrderStatus.Completed)
            .OrderByDescending(o => o.CreatedAt)
            .Include(o => o.TicketType)
                .ThenInclude(t => t!.Event)
            .Include(o => o.Purchaser!)
            .Include(o => o.Currency)
            .Include(o => o.AffiliateCode)
                .ThenInclude(ac => ac!.Channel)
            .Select(o => new OrderBasicBusinessDto(o))
            .AsSplitQuery()
            .ToPagedResult(page, pageSize);

        return orders;
    }

    public async Task<OrderDetailedBusinessDto> GetOrderAsBusinessDto(Guid id)
    {
        var orderEntity = await _dbContext.Orders
            .Include(o => o.TicketType)
                .ThenInclude(t => t!.Event)
            .Include(o => o.Purchaser!)
            .Include(o => o.Currency)
            .Include(o => o.AffiliateCode)
                .ThenInclude(ac => ac!.Channel)
            .Include(o => o.Tickets)
            .AsSplitQuery()
            .FirstOrThrowAsync(o => o.Id == id);

        return new OrderDetailedBusinessDto(orderEntity);
    }

    public async Task<PagedResultDto<OrderBasicDto>> GetOrdersForUserAsDtos(string userId,
        List<Enums.OrderStatus>? statusFilter, Enums.EventStatus? eventStatus, int? eventId, int page, int pageSize)
    {
        var query = _dbContext.Orders
            .Where(o => o.PurchaserId == userId)
            .Include(o => o.TicketType!.Event!.Image)
            .Include(o => o.TicketType!.Event!.Category)
            .Include(o => o.Currency)
            .AsQueryable();

        if (statusFilter != null && statusFilter.Any())
        {
            query = query.Where(o => statusFilter.Contains(o.Status));
        }

        if (eventId != null)
        {
            query = query.Where(o => o.EventId == eventId);
        }

        query = eventStatus switch
        {
            Enums.EventStatus.Upcoming => query.Where(o => o.TicketType!.Event!.EndDate >= DateTimeOffset.UtcNow),
            Enums.EventStatus.Past => query.Where(o => DateTimeOffset.UtcNow >= o.TicketType!.Event!.EndDate),
            _ => query
        };

        return await query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderBasicDto(o))
            .AsSplitQuery()
            .ToPagedResult(page, pageSize);
    }

    public async Task<OrderDetailedDto> GetOrderForUserAsDto(string userId, Guid orderId)
    {
        return await _dbContext.Orders
            .Where(o => o.PurchaserId == userId && o.Id == orderId)
            .Include(o => o.TicketType!.Event!.Business)
            .Include(o => o.TicketType!.Event!.Image)
            .Include(o => o.TicketType!.Event!.TicketBackgroundImage)
            .Include(o => o.TicketType!.Event!.Category)
            .Include(o => o.Currency)
            .Select(o => new OrderDetailedDto(o, _magicService.CreateMagicForOrder(orderId)))
            .FirstOrThrowAsync();
    }

    public async Task<AmountDto> GetOrdersTicketTypeRevenueAsDto(Guid ticketTypeId)
    {
        var revenue = new AmountDto();
        await _dbContext.Orders
            .Where(o => o.TicketTypeId == ticketTypeId)
            .Where(o => o.Status == Enums.OrderStatus.Completed)
            .ForEachAsync(o => revenue.AddOrder(o));
        return revenue;
    }

    public async Task<AmountDto> GetOrdersEventRevenueAsDto(int eventId)
    {
        var revenue = new AmountDto();
        await _dbContext.Orders
            .Where(o => o.EventId == eventId)
            .Where(o => o.Status == Enums.OrderStatus.Completed)
            .ForEachAsync(o => revenue.AddOrder(o));
        return revenue;
    }

    public async Task<PagedResultDto<EventBasicDto>> GetOrdersForUserByEventAsDtos(string userId, List<Enums.OrderStatus>? statusFilter, Enums.EventStatus? eventStatus, int page, int pageSize)
    {
        var base_query = _dbContext.Orders
            .Where(o => o.PurchaserId == userId)
            .Include(o => o.TicketType!.Event!.Image)
            .Include(o => o.TicketType!.Event!.Category)
            .AsQueryable();

        if (statusFilter != null && statusFilter.Any())
        {
            base_query = base_query.Where(o => statusFilter.Contains(o.Status));
        }

        var query = base_query
            .Select(o => o.Event!)
            .Distinct();

        query = eventStatus switch
        {
            Enums.EventStatus.Upcoming => query.Where(e => e.EndDate >= DateTimeOffset.UtcNow),
            Enums.EventStatus.Past => query.Where(e => DateTimeOffset.UtcNow >= e.EndDate),
            _ => query
        };

        return await query
            .Select(e => new EventBasicDto(e))
            .AsSplitQuery()
            .ToPagedResult(page, pageSize);
    }
}
