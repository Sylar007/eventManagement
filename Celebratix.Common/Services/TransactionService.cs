using AutoMapper;
using Celebratix.Common.ErrorHandling;
using Celebratix.Common.Extensions;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Business;
using Celebratix.Common.Models.DTOs.Business.Transaction;
using FluentResults;
using FluentValidation;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Celebratix.Common.Services;

public class TransactionService
{
    private readonly CelebratixDbContext _dbContext;
    private readonly ILogger<TransactionService> _logger;
    private readonly IValidator<TransactionCreateBusinessRequest> _transactionCreateBusinessRequestValidator;
    private IMapper _mapper;

    public TransactionService(CelebratixDbContext dbContext, ILogger<TransactionService> logger,
        IValidator<TransactionCreateBusinessRequest> transactionCreateBusinessRequestValidator, IMapper mapper)
    {
        _dbContext = dbContext;
        _logger = logger;
        _transactionCreateBusinessRequestValidator = transactionCreateBusinessRequestValidator;
        _mapper = mapper;
    }

    public async Task<Result<TransactionBusinessDto>> GetTransactionDto(Guid id)
    {
        var tracking = await _dbContext.Transactions
            .Include(ts => ts.Channel)
            .Include(ts => ts.Event)
            .FirstOrDefaultAsync(ts => ts.Id == id);

        if (tracking == null)
        {
            return Result.Fail(new CelebratixError(ErrorCode.CelebratixGeneric,
                $"Transaction with id {id} could not be found."));
        }

        return new TransactionBusinessDto(tracking);
    }

    public async Task<Result<PagedResultDto<TransactionBusinessDto>>> GetTransactions(Guid businessId, int page, int pageSize)
    {
        return await _dbContext.Transactions
            .Include(ts => ts.Channel)
            .Include(ts => ts.Event)
            .Include(ts => ts.Tracking)
            .Where(ts => ts.BusinessId == businessId)
            .Select(ts => new TransactionBusinessDto(ts))
            .ToPagedResult(page, pageSize);
    }

    public async Task<Result<string>> CreateTransaction(Guid businessId, TransactionCreateBusinessRequest createRequest)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var validationResult = await _transactionCreateBusinessRequestValidator.ValidateAsync(createRequest);

            if (!validationResult.IsValid)
            {
                return Result.Fail(validationResult.Errors.ToErrorMessage());
            }

            var transactionData = _mapper.Map<TransactionCreateBusinessRequest, Transaction>(createRequest);
            transactionData.BusinessId = businessId;

            _dbContext.Add(transactionData);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Created transaction for business: {businessId} with id {transactionData.Id}");
            return transactionData.Id.ToString();
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

    public async Task<Result<TransactionBusinessDto>> UpdateTransaction(Guid transactionId, TransactionCreateBusinessRequest updateRequest)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var validationResult = await _transactionCreateBusinessRequestValidator.ValidateAsync(updateRequest);

            if (!validationResult.IsValid)
            {
                return Result.Fail(validationResult.Errors.ToErrorMessage());
            }

            var transactionData = await _dbContext.Transactions
            .Include(ts => ts.Channel)
            .Include(ts => ts.Event)
            .FirstOrDefaultAsync(ts => ts.Id == transactionId);

            if (transactionData == null)
            {
                return Result.Fail(new CelebratixError(ErrorCode.CelebratixGeneric,
                    $"Transaction with id {transactionId} could not be found."));
            }

            _dbContext.Entry(transactionData).CurrentValues.SetValues(updateRequest);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Updated transaction with id {transactionData.Id}");
            return new TransactionBusinessDto(transactionData);
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

    public async Task<Result<PagedResultDto<TransactionBusinessDto>>> GetTransactionsBySearch(TransactionSearchRequest? searchRequest,
        Guid businessId)
    {
        if (searchRequest == null)
        {
            return await _dbContext.Transactions
                .Include(ts => ts.Channel)
                .Include(ts => ts.Event)
                .Include(ts => ts.Tracking)
                .Where(ts => ts.BusinessId == businessId)
                .Select(ts => new TransactionBusinessDto(ts))
                .ToPagedResult(1, 10);
        }

        var querySearch = GetTransactionSearch(searchRequest);
        var queryFilter = GetFilterQuery(searchRequest);
        var querySort = GetSortQuery(searchRequest);

        return await querySort
            .Include(ts => ts.Channel)
            .Include(ts => ts.Event)
            .Include(ts => ts.Tracking)
            .Where(e => e.BusinessId == businessId)
            .Where(querySearch)
            .Where(queryFilter)
            .Select(e => new TransactionBusinessDto(e))
            .ToPagedResult(searchRequest.Page, searchRequest.PageSize);
    }

    private static ExpressionStarter<Transaction> GetTransactionSearch(TransactionSearchRequest searchRequest)
    {
        var predicate = PredicateBuilder.New<Transaction>(false);

        if (!string.IsNullOrEmpty(searchRequest.SearchText))
        {
            predicate = predicate.And(e => e.FullName.ToLower().Contains(searchRequest.SearchText.ToLower()));
        }

        return predicate;
    }

    private static ExpressionStarter<Transaction> GetFilterQuery(TransactionSearchRequest searchRequest)
    {
        var predicate = PredicateBuilder.New<Transaction>(false);

        if (searchRequest.TransactionDateFrom != null & searchRequest.TransactionDateTo != null)
        {
            predicate = predicate.Or(e => e.TransactionDate >= searchRequest.TransactionDateFrom &&
                                     e.TransactionDate <= searchRequest.TransactionDateTo);
        }
        if (searchRequest.Statuses != null)
        {
            foreach (var status in searchRequest.Statuses)
            {
                predicate = predicate.Or(e => e.Status == status);
            }
        }
        if (searchRequest.ChannelId != null)
        {
            predicate = predicate.Or(e => e.ChannelId == searchRequest.ChannelId);
        }
        if (searchRequest.TrackingId != null)
        {
            predicate = predicate.Or(e => e.TrackingId == searchRequest.TrackingId);
        }

        return predicate;
    }

    private IQueryable<Transaction> GetSortQuery(TransactionSearchRequest searchRequest)
    {
        IQueryable<Transaction> transactions;

        switch (searchRequest.SortColumn)
        {
            case Enums.TransactionSearchSortColumn.TransactiondateAsc:
                transactions = _dbContext.Transactions.AsQueryable().OrderBy(e => e.TransactionDate);
                break;
            case Enums.TransactionSearchSortColumn.TransactiondateDesc:
                transactions = _dbContext.Transactions.AsQueryable().OrderByDescending(e => e.TransactionDate);
                break;
            case Enums.TransactionSearchSortColumn.CreatedAtAsc:
                transactions = _dbContext.Transactions.AsQueryable().OrderBy(e => e.CreatedAt);
                break;
            case Enums.TransactionSearchSortColumn.CreatedAtDesc:
                transactions = _dbContext.Transactions.AsQueryable().OrderByDescending(e => e.CreatedAt);
                break;
            default:
                transactions = _dbContext.Transactions.AsQueryable().OrderByDescending(e => e.CreatedAt);
                break;
        }

        return transactions;
    }
}
