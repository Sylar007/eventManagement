using System.Security.Claims;
using Celebratix.Common.Exceptions;
using Celebratix.Common.Extensions;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Admin.Users;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers.Admin;

/// <summary>
/// Admin specific endpoints for handling users
/// </summary>
[Area("admin")]
[Route("[area]/[controller]")]
[ApiController]
[AuthorizeRoles(Enums.Role.SuperAdmin)]
public class UsersController : ControllerBase
{
    private readonly AccountService _accountService;
    private readonly UserService _userService;

    public UsersController(AccountService accountService, UserService userService)
    {
        _accountService = accountService;
        _userService = userService;
    }

    /// <summary>
    /// Returns all users, including admins
    /// </summary>
    [HttpGet]
    public async Task<PagedResultDto<UserBasicAdminDto>> GetAllUsers(int page = 1, int pageSize = 20)
    {
        return await _userService.GetAllUsersAsAdminDtos(page, pageSize);
    }

    [HttpGet("admins")]
    public async Task<List<UserBasicAdminDto>> GetAllAdminUsers()
    {
        return await _userService.GetAllAdminUsersAsAdminDtos();
    }

    /// <response code="404">If the account was not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDetailedAdminDto>> GetUser(string id)
    {
        return await _userService.GetUserAsDetailedAdminDto(id);
    }

    /// <summary>
    /// Checks if the email is unused/free/unoccupied
    /// Returns a bool with the answer
    /// </summary>
    [HttpGet("check-email-free")]
    public async Task<IActionResult> CheckEmailFree(string email)
    {
        await _userService.VerifyEmailIsFree(email);
        return Ok();
    }

    /// <response code="400">If the supplied role isn't an admin role (unsupported_role)</response>
    [HttpPost("admin")]
    public async Task<UserDetailedAdminDto> CreateAdminUser(CreateAdminUserAdminDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email)!;

        if (!await _accountService.VerifyPassword(currentUserId, dto.CurrentPassword))
        {
            throw new InvalidPasswordException("The supplied password is invalid");
        }

        return await _userService.CreateAdminUser(dto, currentUserEmail);
    }

    /// <response code="403">Owner or location has to be set</response>
    [HttpPost("business")]
    public async Task<UserDetailedAdminDto> CreateBusinessUser(CreateBusinessUserAdminDto dto)
    {
        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email)!;

        return await _userService.CreateBusinessUserAsAdmin(dto, currentUserEmail);
    }

    [HttpPost("send-account-setup-email/{userId}")]
    [AuthorizeRoles(Enums.Role.SuperAdmin)]
    public async Task<IActionResult> SendAccountSetupEmail(string userId)
    {
        await _userService.SendAccountSetupEmail(userId);
        return Ok();
    }

    /// <summary>
    /// Adds the user to the specified role
    /// </summary>
    /// <response code="400">If the action failed and the user wasn't added to the role</response>
    /// <response code="400">If the supplied password was invalid (invalid_password)</response>
    /// <response code="400">If the supplied role isn't an admin role (unsupported_role)</response>
    [HttpPost("roles/add/{userId}")]
    public async Task<IActionResult> AddRole(string userId, Enums.Role role, PasswordVerificationDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (!await _accountService.VerifyPassword(currentUserId, dto.CurrentPassword))
        {
            throw new InvalidPasswordException("The supplied password is invalid");
        }

        var result = await _userService.AddAdminRoleToUser(userId, role);

        if (result.Succeeded)
        {
            return Ok();
        }

        return Problem(
            statusCode: 400,
            type: result.Errors.Select(x => x.Code).ToCommaSeparatedString(),
            detail: result.Errors.Select(x => x.Description).ToCommaSeparatedString()
        );
    }

    /// <summary>
    /// Removes the user from the specified role
    /// </summary>
    /// <response code="400">If the action failed and the user wasn't removed from the role</response>
    /// <response code="400">If the supplied password was invalid (invalid_password)</response>
    /// <response code="400">If the supplied role isn't an admin role (unsupported_role)</response>
    [HttpPost("roles/delete/{userId}")]
    public async Task<IActionResult> RemoveRole(string userId, Enums.Role role, PasswordVerificationDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (!await _accountService.VerifyPassword(currentUserId, dto.CurrentPassword))
        {
            throw new InvalidPasswordException("The supplied password is invalid");
        }

        var result = await _userService.RemoveAdminRoleFromUser(userId, role);

        if (result.Succeeded)
        {
            return Ok();
        }

        return Problem(
            statusCode: 400,
            type: result.Errors.Select(x => x.Code).ToCommaSeparatedString(),
            detail: result.Errors.Select(x => x.Description).ToCommaSeparatedString()
        );
    }
}
