using Asp.Versioning;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Business;
using Celebratix.Common.Services;
using Celebratix.Models;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Errors.Model;

namespace Celebratix.Controllers.Business.V2;

[ApiVersion(2.0)]
[Area("business")]
[Route("v{version:apiVersion}/[area]/[controller]")]
[ApiController]
[AuthorizeRoles(Enums.Role.Business)]
public class TrackingController : CelebratixControllerBase
{
    private readonly TrackingService _trackingService;
    private readonly UserService _userService;

    public TrackingController(UserService userService, TrackingService TrackingService)
    {
        _userService = userService;
        _trackingService = TrackingService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(OperationResult<PagedResultDto<BusinessTrackerDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<PagedResultDto<BusinessTrackerDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTrackings(int page = 1, int pageSize = 10)
    {
        var businessId = await _userService.GetUserBusinessIdByClaimsPrincipal(User);
        if (businessId.IsFailed)
        {
            return GetOkResult(businessId);
        }
        var result = await _trackingService.GetTrackings(businessId.Value, page, pageSize);

        return GetOkResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OperationResult<BusinessTrackerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<BusinessTrackerDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTrackingById(Guid id)
    {
        var businessId = await _userService.GetUserBusinessIdByClaimsPrincipal(User);
        if (businessId.IsFailed)
        {
            return GetOkResult(businessId);
        }
        var result = await _trackingService.GetTrackingDto(id);

        return GetOkResult(result);
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(OperationResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTracking([FromBody] BusinessTrackerCreateRequest createRequest)
    {
        var businessId = await _userService.GetUserBusinessIdByClaimsPrincipal(User);
        if (businessId.IsFailed)
        {
            return GetOkResult(businessId);
        }
        var result = await _trackingService.CreateTracking(businessId.Value, createRequest);

        return GetOkResult(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(OperationResult<BusinessTrackerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<BusinessTrackerDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTracking(Guid id, BusinessTrackerCreateRequest updateRequest)
    {
        var businessId = await _userService.GetUserBusinessIdByClaimsPrincipal(User);
        if (businessId.IsFailed)
        {
            return GetOkResult(businessId);
        }
        var result = await _trackingService.UpdateTracking(id, updateRequest);

        return GetOkResult(result);
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(OperationResult<PagedResultDto<BusinessTrackerDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<PagedResultDto<BusinessTrackerDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTrackingsBySearch(BusinessTrackerSearchRequest? searchRequest)
    {
        var businessId = await _userService.GetUserBusinessIdByClaimsPrincipal(User);
        if (businessId.IsFailed)
        {
            return GetOkResult(businessId);
        }
        var result = await _trackingService.GetTrackingsBySearch(searchRequest, businessId.Value);

        return GetOkResult(result);
    }
}
