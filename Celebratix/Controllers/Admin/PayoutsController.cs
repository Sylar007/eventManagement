using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Admin.Payouts;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers.Admin;

[Area("admin")]
[Route("[area]/[controller]")]
[ApiController]
[AuthorizeRoles(Enums.Role.SuperAdmin)]
public class PayoutsController : ControllerBase
{
    private readonly BusinessPayoutService _service;

    public PayoutsController(BusinessPayoutService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<PagedResultDto<BusinessPayoutAdminDto>> GetPayouts(int page = 1, int pageSize = 8)
    {
        return await _service.GetPayoutsAsAdminDtos(page, pageSize);
    }

    /// <summary>
    /// Returns an empty result if no business for the id was found
    /// </summary>
    [HttpGet("/business/{businessId:guid}")]
    public async Task<PagedResultDto<BusinessPayoutAdminDto>> GetPayoutsForBusiness(Guid businessId, int page = 1, int pageSize = 8)
    {
        return await _service.GetPayoutsAsAdminDtos(businessId, page, pageSize);
    }

    [HttpPost("/business/{businessId:guid}")]
    public async Task<ActionResult<BusinessPayoutAdminDto>> CreatePayout(Guid businessId, BusinessPayoutUpdateAdminDto dto)
    {
        return await _service.CreatePayout(businessId, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BusinessPayoutAdminDto>> UpdatePayout(Guid id, BusinessPayoutUpdateAdminDto dto)
    {
        return await _service.UpdatePayout(id, dto);
    }

}
