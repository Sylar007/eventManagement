using AutoMapper;
using Celebratix.Common.ErrorHandling;
using Celebratix.Common.Extensions;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Business;
using FluentResults;
using FluentValidation;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Celebratix.Common.Services;

public class TrackingService
{
    private readonly CelebratixDbContext _dbContext;
    private readonly ILogger<TrackingService> _logger;

    private readonly IValidator<BusinessTrackerCreateRequest> _trackingCreateRequestValidator;
    private IMapper _mapper;
    public TrackingService(CelebratixDbContext dbContext, ILogger<TrackingService> logger,
        IValidator<BusinessTrackerCreateRequest> trackingCreateRequestValidator, IMapper mapper)
    {
        _dbContext = dbContext;
        _logger = logger;
        _trackingCreateRequestValidator = trackingCreateRequestValidator;
        _mapper = mapper;
    }

    public async Task<Result<BusinessTrackerDto>> GetTrackingDto(Guid id)
    {
        var tracking = await _dbContext.Trackings
            .Include(ts => ts.Channel)
            .Include(ts => ts.Affiliate)
            .FirstOrDefaultAsync(ts => ts.Id == id);

        if (tracking == null)
        {
            return Result.Fail(new CelebratixError(ErrorCode.CelebratixGeneric,
                $"Tracking with id {id} could not be found."));
        }

        return new BusinessTrackerDto(tracking);
    }

    public async Task<Result<PagedResultDto<BusinessTrackerDto>>> GetTrackings(Guid businessId, int page, int pageSize)
    {
        var businessTrackers = await _dbContext.Trackings
            .AsNoTracking()
            .Include(ts => ts.Channel)
            .Include(ts => ts.Affiliate)
            .Where(ts => ts.BusinessId == businessId)
            .Select(ts => new BusinessTrackerDto(ts))
            .ToPagedResult(page, pageSize);

        return await GetTrackingAdditionalInfo(businessTrackers, businessId);
    }

    public async Task<Result<string>> CreateTracking(Guid businessId, BusinessTrackerCreateRequest createRequest)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var validationResult = await _trackingCreateRequestValidator.ValidateAsync(createRequest);

            if (!validationResult.IsValid)
            {
                return Result.Fail(validationResult.Errors.ToErrorMessage());
            }

            var tracking = _mapper.Map<BusinessTrackerCreateRequest, Tracking>(createRequest);
            tracking.BusinessId = businessId;
            _dbContext.Add(tracking);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Created tracking for business: {businessId} with id {tracking.Id}");
            return tracking.Id.ToString();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await transaction.CommitAsync();
        }
    }

    public async Task<Result<BusinessTrackerDto>> UpdateTracking(Guid trackingId, BusinessTrackerCreateRequest updateRequest)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var validationResult = await _trackingCreateRequestValidator.ValidateAsync(updateRequest);

            if (!validationResult.IsValid)
            {
                return Result.Fail(validationResult.Errors.ToErrorMessage());
            }

            var tracking = await _dbContext.Trackings
            .Include(ts => ts.Channel)
            .Include(ts => ts.Affiliate)
            .FirstOrDefaultAsync(ts => ts.Id == trackingId);

            if (tracking == null)
            {
                return Result.Fail(new CelebratixError(ErrorCode.CelebratixGeneric,
                    $"Tracking with id {trackingId} could not be found."));
            }
            _dbContext.Entry(tracking).CurrentValues.SetValues(updateRequest);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Updated tracking for business with id {tracking.Id}");
            return new BusinessTrackerDto(tracking);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await transaction.CommitAsync();
        }
    }

    public async Task<Result<PagedResultDto<BusinessTrackerDto>>> GetTrackingsBySearch(BusinessTrackerSearchRequest? searchRequest,
        Guid businessId)
    {
        if (searchRequest == null)
        {
            return await _dbContext.Trackings
                .Include(e => e.Channel)
                .Include(e => e.Affiliate)
                .Where(ts => ts.BusinessId == businessId)
                .Select(ts => new BusinessTrackerDto(ts))
                .ToPagedResult(1, 10);
        }

        var querySearch = GetTrackingSearch(searchRequest);
        var queryFilter = GetFilterQuery(searchRequest);
        var querySort = GetSortQuery(searchRequest);

        return await querySort
            .Include(e => e.Channel)
            .Include(e => e.Affiliate)
            .Where(e => e.BusinessId == businessId)
            .Where(querySearch)
            .Where(queryFilter)
            .Select(e => new BusinessTrackerDto(e))
            .ToPagedResult(searchRequest.Page, searchRequest.PageSize);
    }

    private static ExpressionStarter<Tracking> GetTrackingSearch(BusinessTrackerSearchRequest searchRequest)
    {
        var predicate = PredicateBuilder.New<Tracking>(false);

        if (!string.IsNullOrEmpty(searchRequest.SearchText))
        {
            predicate = predicate.And(e => e.Name.ToLower().Contains(searchRequest.SearchText.ToLower()));
        }

        return predicate;
    }
    private static ExpressionStarter<Tracking> GetFilterQuery(BusinessTrackerSearchRequest searchRequest)
    {
        var predicate = PredicateBuilder.New<Tracking>(false);

        if (searchRequest.Affiliates != null)
        {
            foreach (var affiliate in searchRequest.Affiliates)
            {
                predicate = predicate.Or(e => e.AffiliateId == affiliate);
            }
        }
        if (searchRequest.Channel != null)
        {
            predicate = predicate.Or(e => e.ChannelId == searchRequest.Channel);
        }

        return predicate;
    }
    private IQueryable<Tracking> GetSortQuery(BusinessTrackerSearchRequest searchRequest)
    {
        IQueryable<Tracking> trackings;

        switch (searchRequest.SortColumn)
        {
            case Enums.TrackingSearchSortColumn.NameAsc:
                trackings = _dbContext.Trackings.AsQueryable().OrderBy(e => e.Name);
                break;
            case Enums.TrackingSearchSortColumn.NameDesc:
                trackings = _dbContext.Trackings.AsQueryable().OrderByDescending(e => e.Name);
                break;
            case Enums.TrackingSearchSortColumn.CreatedAtAsc:
                trackings = _dbContext.Trackings.AsQueryable().OrderBy(e => e.CreatedAt);
                break;
            case Enums.TrackingSearchSortColumn.CreatedAtDesc:
                trackings = _dbContext.Trackings.AsQueryable().OrderByDescending(e => e.CreatedAt);
                break;
            default:
                trackings = _dbContext.Trackings.AsQueryable().OrderByDescending(e => e.CreatedAt);
                break;
        }

        return trackings;
    }

    private async Task<Result<PagedResultDto<BusinessTrackerDto>>> GetTrackingAdditionalInfo(Result<PagedResultDto<BusinessTrackerDto>> businessTrackers, Guid businessId)
    {
        foreach (var businessTracker in businessTrackers.Value.List)
        {
            var ticketTypes = await _dbContext.ChannelEventTicketTypes
                    .Include(c => c.ChannelEvent)
                    .Where(c => c.ChannelEvent.ChannelId == businessTracker.Channel.Id)
                    .Include(c => c.EventTicketType)
                    .ToListAsync();

            int sumVisits = 0;
            int sumTickets = 0;
            decimal sumRevenue = 0;

            foreach (var ticketType in ticketTypes)
            {
                var calculateVisits = _dbContext.EventTicketTypes
                    .Where(c => c.Id == ticketType.EventTicketTypeId)
                    .Sum(s => s.TicketsCheckedIn);
                sumVisits += calculateVisits;
                var calculateTickets = _dbContext.EventTicketTypes
                    .Where(c => c.Id == ticketType.EventTicketTypeId)
                    .Sum(s => s.TicketsSold);
                sumTickets += calculateTickets;

                var orders = _dbContext.Orders
                    .Where(o => o.TicketType!.Event!.BusinessId == businessId)
                    .Where(o => o.MarketplaceListingId == null &&
                                o.TicketTransferOfferId == null) // I.e. it's a primary market order
                    .Where(o => o.Status == Enums.OrderStatus.Completed)
                    .AsEnumerable();
                var revenue = new AmountDto();
                foreach (var order in orders)
                {
                    revenue.AddOrder(order);
                }

                sumRevenue += revenue.Base + revenue.ServiceFee + revenue.ApplicationFee;
            }
            businessTracker.Visits = sumVisits;
            businessTracker.Tickets = sumTickets;
            businessTracker.Revenue = sumRevenue;
        }
        return businessTrackers;
    }
}
