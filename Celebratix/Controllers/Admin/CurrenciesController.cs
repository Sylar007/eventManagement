using System.Security.Claims;
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
public class CurrenciesController : ControllerBase
{
    private readonly CurrencyService _service;

    public CurrenciesController(CurrencyService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gives back a list of all countries (both active and inactive)
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<CurrencyAdminDto>> GetCurrencies()
    {
        return await _service.GetCurrenciesAsAdminDtos();
    }

    /// <response code="404">No currency with given code was found</response>
    [HttpGet("{code}")]
    public async Task<ActionResult<CurrencyAdminDto>> GetCurrency(string code)
    {
        return await _service.GetCurrencyAsAdminDto(code);
    }

    [HttpPost]
    public async Task<ActionResult<CurrencyAdminDto>> CreateCurrency(CurrencyCreateAdminDto dto)
    {
        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email)!;
        return await _service.CreateCurrency(dto, currentUserEmail);
    }

    /// <response code="404">No currency with given id was found</response>
    [HttpPut("{code}")]
    public async Task<ActionResult<CurrencyAdminDto>> UpdateCurrency(string code, CurrencyUpdateAdminDto dto)
    {
        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email)!;
        return await _service.EditCurrency(code, dto, currentUserEmail);
    }
}
