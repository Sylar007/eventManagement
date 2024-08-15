using Celebratix.Common.Extensions;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Celebratix.Common.Services;

public class CurrencyService
{
    private readonly CelebratixDbContext _dbContext;
    private readonly ILogger<CurrencyService> _logger;

    public CurrencyService(CelebratixDbContext dbContext, ILogger<CurrencyService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ICollection<CurrencyAdminDto>> GetCurrenciesAsAdminDtos()
    {
        return await _dbContext.Currencies
            .Select(c => new CurrencyAdminDto(c))
            .ToListAsync();
    }

    public async Task<ICollection<CurrencyDto>> GetEnabledCurrenciesAsDto()
    {
        return await _dbContext.EnabledCurrencies
            .Select(c => new CurrencyDto(c))
            .ToListAsync();
    }

    public async Task<CurrencyAdminDto> GetCurrencyAsAdminDto(string code)
    {
        var currency = await _dbContext.Currencies
            .FirstOrThrowAsync(c => c.Code == code);
        return new CurrencyAdminDto(currency);
    }

    public async Task<CurrencyDto> GetCurrencyAsDto(string code)
    {
        var currency = await _dbContext.Currencies
            .FirstOrThrowAsync(c => c.Code == code);
        return new CurrencyDto(currency);
    }

    public async Task<CurrencyAdminDto> CreateCurrency(CurrencyCreateAdminDto dto, string adminEmail)
    {
        var currency = new Currency();
        _dbContext.Entry(currency).CurrentValues.SetValues(dto);
        _dbContext.Add(currency);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Currency with name {CurrencyName} and code: {CurrencyCode} was successfully created\n" +
                               "Created by admin with email: {AdminEmail}", currency.Name, currency.Code, adminEmail);
        return new CurrencyAdminDto(currency);
    }

    public async Task<CurrencyAdminDto> EditCurrency(string code, CurrencyUpdateAdminDto dto, string adminEmail)
    {
        var currency = await _dbContext.Currencies
            .FirstOrThrowAsync(c => c.Code == code);

        _dbContext.Entry(currency).CurrentValues.SetValues(dto);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Currency with name {CurrencyName} and code: {CurrencyCode} was successfully edited\n" +
                               "Edited by admin with email: {AdminEmail}", currency.Name, currency.Code, adminEmail);
        return new CurrencyAdminDto(currency);
    }
}
