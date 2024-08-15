using System.Security.Claims;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Business.Events;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers.Business;

[Area("business")]
[Route("[area]")]
[ApiController]
[AuthorizeRoles(Enums.Role.Business)]
public class AffiliateCodesController : ControllerBase
{
    private readonly AffiliateCodeService _affiliateCodeService;
    private readonly UserService _userService;

    public AffiliateCodesController(UserService userService, AffiliateCodeService affiliateCodeService)
    {
        _userService = userService;
        _affiliateCodeService = affiliateCodeService;
    }

    [HttpGet("[controller]")]
    public async Task<PagedResultDto<AffliateBusinessDto>> GetAffiliateCodes(int page, int pageSize)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var businessId = await _userService.GetUserBusinessId(userId);
        return await _affiliateCodeService.GetAffiliateCodesForBusiness(businessId, page, pageSize);
    }

    [HttpGet("/business/channel/{channelId:Guid}/[controller]")]
    public async Task<PagedResultDto<AffliateBusinessDto>> GetAffiliateCodesForChannel(Guid channelId, int page, int pageSize)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasChannelAccess(userId, channelId);
        return await _affiliateCodeService.GetAffiliateCodesForChannel(channelId, page, pageSize);
    }

    [HttpPost("[controller]")]
    public async Task<ActionResult<AffliateDetailedBusinessDto>> CreateAffiliateCode(AffliateCreateBusinessDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasChannelAccess(userId, dto.ChannelId);
        return await _affiliateCodeService.CreateAffiliateCode(userId, dto);
    }

    [HttpPut("[controller]/{id:guid}")]
    public async Task<ActionResult<AffliateDetailedBusinessDto>> UpdateAffiliateCode(Guid id, AffliateUpdateBusinessDto dto)
    {
        var channelId = (await _affiliateCodeService.GetAffiliateCode(id)).Channel!.Id;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.VerifyUserHasChannelAccess(userId, channelId);
        return await _affiliateCodeService.UpdateAffiliateCode(id, dto);
    }
}
