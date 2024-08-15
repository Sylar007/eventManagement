using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs.Admin;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers.Admin;

/// <summary>
/// Admin specific endpoints for managing countries
/// See "public" CountriesController for non-admin endpoints
/// </summary>
[Area("admin")]
[Route("[area]/[controller]")]
[ApiController]
[AuthorizeRoles(Enums.Role.SuperAdmin)]
public class CountriesController : ControllerBase
{
    private readonly CountryService _service;

    public CountriesController(CountryService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gives back a list of all countries (both active and inactive)
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<CountryAdminDto>> GetCountries()
    {
        return await _service.GetCountriesAsAdminDtos();
    }

    /// <response code="404">No country with given id was found</response>
    [HttpPost("{id}/enable")]
    public async Task<ActionResult> EnableCountry(string id)
    {
        await _service.EnableCountry(id);

        return Ok();
    }

    /// <response code="404">No country with given id was found</response>
    [HttpPost("{id}/disable")]
    public async Task<ActionResult> DisableCountry(string id)
    {
        await _service.DisableCountry(id);

        return Ok();
    }
}
