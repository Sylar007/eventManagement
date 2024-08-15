using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Business.Channel;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Errors.Model;
using System.Security.Claims;

namespace Celebratix.Controllers.Business;

[Area("business")]
[Route("[area]/[controller]")]
[ApiController]
[AuthorizeRoles(Enums.Role.Business)]
public class ChannelsController : ControllerBase
{
    private readonly ChannelService _channelService;
    private readonly UserService _userService;

    public ChannelsController(UserService userService, ChannelService ChannelService)
    {
        _userService = userService;
        _channelService = ChannelService;
    }

    [HttpGet]
    public async Task<PagedResultDto<ChannelBusinessDto>> GetChannels(int page = 1, int pageSize = 10)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var businessId = await _userService.GetUserBusinessId(userId);
        return await _channelService.GetChannels(businessId, page, pageSize);
    }

    [HttpGet("{id:guid}")]
    public async Task<ChannelBusinessDto> GetChannelById(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasChannelAccess(userId, id);
        return await _channelService.GetChannelDto(id);
    }

    [HttpPost]
    public async Task<ChannelBusinessDto> CreateChannel(ChannelCreateBusinessRequest dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var businessId = await _userService.GetUserBusinessId(userId);
        return await _channelService.CreateChannelDto(businessId, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ChannelBusinessDto> UpdateChannel(Guid id, ChannelCreateBusinessRequest dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var businessId = await _userService.GetUserBusinessId(userId);
        var channel = await _channelService.GetChannel(id);
        if (channel.BusinessId != businessId)
            throw new ForbiddenException("You do not have access to this channel");
        return await _channelService.UpdateChannel(id, dto);
    }
}
