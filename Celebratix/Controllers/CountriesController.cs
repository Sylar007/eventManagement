using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers;

[Route("[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly CountryService _service;

    public CountriesController(CountryService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gives back a list of all active countries
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<CountryDto>> GetCountries()
    {
        return await _service.GetEnabledCountriesAsDtos();
    }

    /// <response code="404">No country with given id was found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<CountryDto>> GetCountry(string id)
    {
        return await _service.GetCountryAsDtoById(id);
    }
}
