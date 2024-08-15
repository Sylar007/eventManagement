using Celebratix.Common.Extensions;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Celebratix.Common.Services;

public class CountryService
{
    private readonly CelebratixDbContext _dbContext;
    private readonly ILogger<CountryService> _logger;

    public CountryService(CelebratixDbContext dbContext, ILogger<CountryService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<CountryDto>> GetEnabledCountriesAsDtos()
    {
        var dtos = await _dbContext.EnabledCountries.Select(c => new CountryDto(c)).ToListAsync();

        return dtos;
    }

    public async Task<CountryDto> GetCountryAsDtoById(string id)
    {
        var dbEntity = await _dbContext.Countries.FirstOrThrowAsync(c => c.Id == id);
        return new CountryDto(dbEntity);
    }

    public async Task<IEnumerable<CountryAdminDto>> GetCountriesAsAdminDtos()
    {
        var dtos = await _dbContext.Countries.Select(c => new CountryAdminDto(c)).ToListAsync();
        return dtos;
    }

    public async Task EnableCountry(string id)
    {
        var country = await _dbContext.Countries.FirstOrThrowAsync(c => c.Id == id);

        country.Enabled = true;

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Country with id: {CountryId} was enabled", country.Id);
    }

    public async Task DisableCountry(string id)
    {
        var country = await _dbContext.Countries.FirstOrThrowAsync(c => c.Id == id);

        country.Enabled = false;

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Country with id: {CountryId} was disabled", country.Id);
    }

    public async Task<Country> GetEnabledCountryById(string id)
    {
        var country = await _dbContext.EnabledCountries.FirstOrThrowAsync(c => c.Id == id);
        return country;
    }

    public async Task VerifyEnabledCountryExists(string id)
    {
        await _dbContext.EnabledCountries.AnyOrThrowAsync(c => c.Id == id);
    }
}
