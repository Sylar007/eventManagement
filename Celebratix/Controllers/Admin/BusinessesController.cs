using System.Security.Claims;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Admin.Business;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers.Admin;

[Area("admin")]
[Route("[area]/[controller]")]
[ApiController]
[AuthorizeRoles(Enums.Role.SuperAdmin)]
public class BusinessesController : ControllerBase
{
    private readonly BusinessService _service;

    public BusinessesController(BusinessService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<PagedResultDto<BusinessBasicAdminDto>> GetCompanies(int page = 1, int pageSize = 8)
    {
        return await _service.GetBusinessesAsAdminDtos(page, pageSize);
    }

    /// <response code="404">No company with given id was found</response>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BusinessDetailedAdminDto>> GetCompany(Guid id)
    {
        return await _service.GetBusinessAsAdminDto(id);
    }

    /// <response code="404">No valid user for the given contact id was found</response>
    [HttpPost]
    public async Task<ActionResult<BusinessDetailedAdminDto>> Create(BusinessUpdateAdminDto dto)
    {
        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email)!;
        return await _service.CreateBusiness(dto, currentUserEmail);
    }

    /// <response code="404">No company with given id was found</response>
    /// <response code="404">No valid user for the given contact id was found</response>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BusinessDetailedAdminDto>> Edit(Guid id, BusinessUpdateAdminDto dto)
    {
        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email)!;
        return await _service.UpdateBusinessAsAdmin(id, dto, currentUserEmail);
    }

    /// <response code="404">No company with given id was found</response>
    [HttpGet("{id:guid}/revenue")]
    public async Task<ActionResult<AmountDto>> GetCompanyRevenue(Guid id, TimespanInputDto dto)
    {
        return await _service.GetBusinessRevenue(id, dto.Start, dto.End);
    }

    // TODO: Managing businesses, remove business etc.
}
