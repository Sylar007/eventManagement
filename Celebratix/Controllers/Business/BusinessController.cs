using System.Security.Claims;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Admin.Business;
using Celebratix.Common.Models.DTOs.Business.Business;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers.Business;

[Area("business")]
[Route("[area]/[controller]")]
[ApiController]
[AuthorizeRoles(Enums.Role.Business, Enums.Role.TicketChecker)]
public class BusinessController : ControllerBase
{
    private readonly BusinessService _businessService;
    private readonly UserService _userService;

    public BusinessController(BusinessService businessService, UserService userService)
    {
        _businessService = businessService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<BusinessDetailedBusinessDto>> GetBusiness()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var businessId = await _userService.GetUserBusinessId(currentUserId);

        return await _businessService.GetBusinessAsBusinessDto(businessId);
    }

    [HttpGet("revenue")]
    [AuthorizeRoles(Enums.Role.Business)]
    public async Task<ActionResult<AmountDto>> GetCompanyRevenue(TimespanInputDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var businessId = await _userService.GetUserBusinessId(currentUserId);
        return await _businessService.GetBusinessRevenue(businessId, dto.Start, dto.End);
    }

    [HttpPut]
    [AuthorizeRoles(Enums.Role.Business)]
    public async Task<ActionResult<BusinessDetailedBusinessDto>> UpdateBusinessAsBusiness(BusinessUpdateBusinessDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var businessId = await _userService.GetUserBusinessId(currentUserId);
        return await _businessService.UpdateBusinessAsBusiness(businessId, dto);
    }
}
