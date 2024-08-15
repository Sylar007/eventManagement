using System.Security.Claims;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Business.Payouts;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers.Business;

[Area("business")]
[Route("[area]/[controller]")]
[ApiController]
[AuthorizeRoles(Enums.Role.Business)]
public class PayoutsController : ControllerBase
{
    private readonly BusinessPayoutService _payoutService;
    private readonly UserService _userService;

    public PayoutsController(BusinessPayoutService payoutService, UserService userService)
    {
        _payoutService = payoutService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<PagedResultDto<BusinessPayoutBusinessDto>> GetPayouts(int page = 1, int pageSize = 8)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var businessId = await _userService.GetUserBusinessId(currentUserId);
        return await _payoutService.GetPayoutsAsBusinessDtos(businessId, page, pageSize);
    }
}
