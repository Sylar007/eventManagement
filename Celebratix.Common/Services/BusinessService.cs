using Celebratix.Common.Extensions;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Admin.Business;
using Celebratix.Common.Models.DTOs.Business.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Celebratix.Common.Services;

public class BusinessService
{
    private readonly CelebratixDbContext _dbContext;
    private readonly ILogger<BusinessService> _logger;

    public BusinessService(CelebratixDbContext dbContext, ILogger<BusinessService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Business> GetBusiness(Guid id)
    {
        var company = await _dbContext.Businesses.FirstOrThrowAsync(c => c.Id == id);
        return company;
    }

    public async Task<PagedResultDto<BusinessBasicAdminDto>> GetBusinessesAsAdminDtos(int page, int pageSize)
    {
        return await _dbContext.Businesses
            .OrderByDescending(c => c.CreatedAt)
            .Include(c => c.Country)
            .Select(c => new BusinessBasicAdminDto(c))
            .ToPagedResult(page, pageSize);
    }

    public async Task<BusinessDetailedAdminDto> GetBusinessAsAdminDto(Guid id)
    {
        var business = await _dbContext.Businesses
            .Include(c => c.Country)
            .FirstOrThrowAsync(c => c.Id == id);
        return new BusinessDetailedAdminDto(business);
    }

    public async Task<BusinessDetailedBusinessDto> GetBusinessAsBusinessDto(Guid id)
    {
        var business = await _dbContext.Businesses
            .Include(c => c.Country)
            .FirstOrThrowAsync(c => c.Id == id);
        return new BusinessDetailedBusinessDto(business);
    }

    public async Task<BusinessDetailedAdminDto> CreateBusiness(BusinessUpdateAdminDto dto, string adminEmail)
    {
        var business = new Business();
        _dbContext.Entry(business).CurrentValues.SetValues(dto);
        _dbContext.Businesses.Add(business);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("New business {Id} created by admin: {AdminEmail}", business.Id, adminEmail);

        return await GetBusinessAsAdminDto(business.Id);
    }

    public async Task<BusinessDetailedAdminDto> UpdateBusinessAsAdmin(Guid id, BusinessUpdateAdminDto dto, string adminEmail)
    {
        var business = await _dbContext.Businesses.FirstOrThrowAsync(c => c.Id == id);

        _dbContext.Entry(business).CurrentValues.SetValues(dto);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Business {Id} updated by admin: {AdminEmail}", business.Id, adminEmail);

        return await GetBusinessAsAdminDto(business.Id);
    }

    public async Task<BusinessDetailedBusinessDto> UpdateBusinessAsBusiness(Guid id, BusinessUpdateBusinessDto dto)
    {
        var business = await _dbContext.Businesses.FirstOrThrowAsync(c => c.Id == id);
        if (dto.EventsPageUrl != null && dto.EventsPageUrl.EndsWith("/"))
            dto.EventsPageUrl = dto.EventsPageUrl.TrimEnd('/');
        _dbContext.Entry(business).CurrentValues.SetValues(dto);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Business {Id} updated by business", business.Id);
        return await GetBusinessAsBusinessDto(business.Id);
    }

    public async Task<ActionResult<AmountDto>> GetBusinessRevenue(Guid id, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        // Just to throw a 404 if the company doesn't exist
        await _dbContext.Businesses.AnyOrThrowAsync(b => b.Id == id);

        var orders = _dbContext.Orders
            .Where(o => o.TicketType!.Event!.BusinessId == id)
            .Where(o => o.MarketplaceListingId == null && o.TicketTransferOfferId == null) // I.e. it's a primary market order
            .Where(o => o.CompletedAt >= startDate && o.CompletedAt <= endDate)
            .Where(o => o.Status == Enums.OrderStatus.Completed)
            .AsAsyncEnumerable();
        var revenue = new AmountDto();
        await foreach (var order in orders)
        {
            revenue.AddOrder(order);
        }

        return revenue;
    }
}
