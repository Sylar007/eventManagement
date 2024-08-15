using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers;

[Route("[controller]")]
[ApiController]
public class CurrenciesController : ControllerBase
{
    private readonly CurrencyService _service;

    public CurrenciesController(CurrencyService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gives back a list of all active currencies
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<CurrencyDto>> GetCurrencies()
    {
        return await _service.GetEnabledCurrenciesAsDto();
    }

    /// <response code="404">No currency with given code was found</response>
    [HttpGet("{code}")]
    public async Task<ActionResult<CurrencyDto>> GetCurrency(string code)
    {
        return await _service.GetCurrencyAsDto(code);
    }
}
