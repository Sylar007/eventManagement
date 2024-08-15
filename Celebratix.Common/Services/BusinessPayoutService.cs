using Celebratix.Common.Extensions;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Admin.Payouts;
using Celebratix.Common.Models.DTOs.Business.Payouts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Celebratix.Common.Services;

public class BusinessPayoutService
{
    private readonly CelebratixDbContext _dbContext;
    private readonly ILogger<BusinessPayoutService> _logger;

    public BusinessPayoutService(CelebratixDbContext dbContext, ILogger<BusinessPayoutService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PagedResultDto<BusinessPayoutBusinessDto>> GetPayoutsAsBusinessDtos(Guid businessId, int page, int pageSize)
    {
        var payouts = await _dbContext.BusinessPayouts
            .Where(p => p.BusinessId == businessId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new BusinessPayoutBusinessDto(p))
            .ToPagedResult(page, pageSize);
        return payouts;
    }

    public async Task<PagedResultDto<BusinessPayoutAdminDto>> GetPayoutsAsAdminDtos(int page, int pageSize)
    {
        var payouts = await _dbContext.BusinessPayouts
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new BusinessPayoutAdminDto(p))
            .ToPagedResult(page, pageSize);
        return payouts;
    }

    public async Task<PagedResultDto<BusinessPayoutAdminDto>> GetPayoutsAsAdminDtos(Guid businessId, int page, int pageSize)
    {
        var payouts = await _dbContext.BusinessPayouts
            .Where(p => p.BusinessId == businessId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new BusinessPayoutAdminDto(p))
            .ToPagedResult(page, pageSize);
        return payouts;
    }

    public async Task<ActionResult<BusinessPayoutAdminDto>> CreatePayout(Guid businessId, BusinessPayoutUpdateAdminDto dto)
    {
        var payout = new BusinessPayout();
        _dbContext.Add(payout);
        payout.BusinessId = businessId;
        _dbContext.Entry(payout).CurrentValues.SetValues(dto);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Payout for business {BusinessId} with id {PayoutId} was created", businessId, payout.Id);
        return new BusinessPayoutAdminDto(payout);
    }

    public async Task<ActionResult<BusinessPayoutAdminDto>> UpdatePayout(Guid id, BusinessPayoutUpdateAdminDto dto)
    {
        var payout = await _dbContext.BusinessPayouts.FirstOrThrowAsync(p => p.Id == id);
        _dbContext.Entry(payout).CurrentValues.SetValues(dto);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Business payout with id {PayoutId} was updated", id);
        return new BusinessPayoutAdminDto(payout);
    }
}
