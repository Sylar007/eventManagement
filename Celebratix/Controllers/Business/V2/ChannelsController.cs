using Asp.Versioning;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Business.Channel;
using Celebratix.Common.Services;
using Celebratix.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Celebratix.Controllers.Business.V2;

[ApiVersion(2.0)]
[Area("business")]
[Route("v{version:apiVersion}/[area]/[controller]")]
[ApiController]
[AuthorizeRoles(Enums.Role.Business)]
public class ChannelsController : CelebratixControllerBase
{
    private readonly ChannelService _channelService;
    private readonly UserService _userService;

    public ChannelsController(UserService userService,
        ChannelService channelService)
    {
        _userService = userService;
        _channelService = channelService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(OperationResult<PagedResultDto<ChannelBusinessDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<PagedResultDto<ChannelBusinessDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetChannels(int page = 1, int pageSize = 10)
    {
        var businessId = await _userService.GetUserBusinessIdByClaimsPrincipal(User);
        if (businessId.IsFailed)
        {
            return GetOkResult(businessId);
        }
        var result = await _channelService.GetChannelsRequest(businessId.Value, page, pageSize);

        return GetOkResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OperationResult<ChannelDetailedDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<ChannelDetailedDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetChannelById(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var accessValidation = await _userService.VerifyUserHasChannelAccessV2(userId, id);
        if (accessValidation.IsFailed)
        {
            return GetOkResult<ChannelBusinessDto>(accessValidation);
        }
        var result = await _channelService.GetChannelRequest(id);

        return GetOkResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OperationResult<ChannelBusinessDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<ChannelBusinessDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateChannel([FromForm] ChannelRequest request)
    {
        var businessId = await _userService.GetUserBusinessIdByClaimsPrincipal(User);
        if (businessId.IsFailed)
        {
            return GetOkResult(businessId);
        }
        var result = await _channelService.CreateChannelV2(businessId.Value, request);

        return GetOkResult(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(OperationResult<ChannelBusinessDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<ChannelBusinessDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateChannel(Guid id, ChannelRequest updateRequest)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var accessValidation = await _userService.VerifyUserHasChannelAccessV2(userId, id);

        if (accessValidation.IsFailed)
        {
            return GetOkResult<ChannelBusinessDto>(accessValidation);
        }

        var result = await _channelService.UpdateChannelV2(id, updateRequest);

        return GetOkResult(result);
    }
}
