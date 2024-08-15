using System.Security.Claims;
using Celebratix.Common.Models.DTOs.User;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers.User;

/// <summary>
/// For managing the "connected account" details needed for payouts
/// (and therefore for creating marketplace listings)
/// </summary>
[Area("user")]
[Route("[area]/[controller]")]
[ApiController]
[Authorize]
public class StripeController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public StripeController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("account/connect")]
    public async Task<ActionResult<bool>> CreateConnectAccount(CreateStripeConnectAccountDto dto)
    {
        var ipAddress = Request.HttpContext.Connection.RemoteIpAddress;
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        _ = await _paymentService.RegisterBankAccountForUser(currentUserId, dto, ipAddress!.ToString());
        return true;
    }

    /// <summary>
    /// Provides a form for inputting outstanding requirements.
    /// Send the user to the form in this mode to just collect the new information you need.
    /// </summary>
    [HttpPost("account/onboarding")]
    public async Task<ActionResult<StripeAccountLinkResponseDto>> CreateOnboardingAccountLink(StripeAccountLinkUrlsDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var responseDto = await _paymentService.CreateStripeOnboardingAccountLink(currentUserId, dto.ReturnUrl, dto.RefreshUrl);
        return responseDto;
    }

    /// <summary>
    /// Displays the fields that are already populated on the account object,
    /// and allows your user to edit previously provided information.
    /// Consider framing this as “edit my profile” or “update my verification information”.
    /// </summary>
    [HttpPost("account/update")]
    public async Task<ActionResult<StripeAccountLinkResponseDto>> CreateUpdateAccountLink(StripeAccountLinkUrlsDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var responseDto = await _paymentService.CreateStripeUpdateAccountLink(currentUserId, dto.ReturnUrl, dto.RefreshUrl);
        return responseDto;
    }
}
