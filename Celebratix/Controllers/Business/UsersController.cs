using System.Security.Claims;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs.Business.Users;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers.Business;

[Area("business")]
[Route("[area]/[controller]")]
[ApiController]
[AuthorizeRoles(Enums.Role.Business)]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<List<UserBasicBusinessDto>> GetAllBusinessUsers()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var businessId = await _userService.GetUserBusinessId(userId!);
        return await _userService.GetAllUsersAsBusinessDtos(businessId);
    }

    /// <response code="404">If the account was not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDetailedBusinessDto>> GetUser(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var businessId = await _userService.GetUserBusinessId(userId!);
        return await _userService.GetUserAsDetailedBusinessDto(businessId, id);
    }

    [HttpPost]
    public async Task<UserDetailedBusinessDto> CreateBusinessUser(CreateBusinessUserDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await _userService.CreateBusinessUserAsBusiness(dto, userId);
    }

    [HttpPost("send-account-setup-email/{userId}")]
    public async Task<IActionResult> SendAccountSetupEmail(string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _userService.SendCompanyAccountSetupEmail(currentUserId, userId);
        return Ok();
    }
}
