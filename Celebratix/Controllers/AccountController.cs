using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Celebratix.Common.Exceptions;
using Celebratix.Common.Extensions;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.User;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers;

[Route("[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;
    private readonly JwtService _jwtService;
    private readonly UserService _userService;

    public AccountController(AccountService accountService, JwtService jwtService, UserService userService)
    {
        _accountService = accountService;
        _jwtService = jwtService;
        _userService = userService;
    }

    [Authorize]
    [HttpGet]
    public async Task<UserDto> GetCurrent(bool? payoutAccounts)
    {
        // User should never be null as the authorize attribute is set
        var user = await _userService
            .GetUserDtoById(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                payoutAccounts);
        return user;
    }

    /// <summary>
    /// Used to refresh the cookie manually.
    /// Main reasons to use this would be to refresh the claims and roles
    /// (i.e. see if the users role has changed. Could be done if the user is waiting to get verified and refreshes the page)
    /// </summary>
    [Authorize]
    [HttpPost("refresh-cookie")]
    public async Task<IActionResult> RefreshCookie()
    {
        // User should never be null as the authorize attribute is set
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // This more or less logins in the user again. So it's very important that the user can't specify the user id themselves
        await _accountService.RefreshCookie(userId);
        return Ok();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _accountService.Logout();
        return Ok();
    }

    [HttpPost("login/phone/token")]
    public async Task<ActionResult<UserDto>> LoginWithPhoneForToken(LoginWithPhoneDto dto)
    {
        var (result, user) = await _accountService.LoginWithPhone(dto.Phone, dto.VerificationCode, dto.StaySignedIn);
        if (!result.Succeeded || user == null)
        {
            return Problem(
                statusCode: 400,
                type: "invalid_login",
                title: result.ToString()
            );
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };

        foreach (var userRole in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole));
        }
        var token = _jwtService.GenerateToken(claims, DateTime.Now.AddDays(14));
        user.Token = new JwtTokenDto(token);
        var refreshToken = await _accountService.GenerateRefreshToken(user.Id, DateTime.Now.AddMonths(1));
        var refresh_claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, refreshToken.Id.ToString()),
            new Claim("OriginatorIdentifier", refreshToken.Originator.ToString()),
        };
        var refreshTokenJwt = _jwtService.GenerateToken(refresh_claims, refreshToken.Expires);
        user.Token = new JwtTokenDto(token);
        user.RefreshToken = new JwtTokenDto(refreshTokenJwt);

        return user;
    }

    [HttpPost("login/email/token")]
    public async Task<ActionResult<UserDto>> LoginWithEmailForToken(LoginWithEmailDto dto)
    {
        var (result, user) = await _accountService.LoginWithEmail(dto.Email, dto.Password, dto.StaySignedIn);
        if (!result.Succeeded || user == null)
        {
            return Problem(
                statusCode: 400,
                type: "invalid_login",
                title: result.ToString()
            );
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };

        foreach (var userRole in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole));
        }
        var token = _jwtService.GenerateToken(claims, DateTime.Now.AddDays(14));
        user.Token = new JwtTokenDto(token);
        var refreshToken = await _accountService.GenerateRefreshToken(user.Id, DateTime.Now.AddMonths(1));
        var refresh_claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, refreshToken.Id.ToString()),
            new Claim("OriginatorIdentifier", refreshToken.Originator.ToString()),
        };
        var refreshTokenJwt = _jwtService.GenerateToken(refresh_claims, refreshToken.Expires);
        user.RefreshToken = new JwtTokenDto(refreshTokenJwt);

        return user;
    }

    /// <response code="400">If the refresh token is invalid (invalid_refresh_token)</response>
    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenPairDto>> RefreshToken(string refreshToken)
    {
        if (!_jwtService.IsValidToken(refreshToken))
            throw new InvalidRefreshTokenException($"Refresh token \"{refreshToken}\" was invalid");
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(refreshToken);
        var refreshTokenId = jwt.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => Guid.Parse(c.Value))
            .First();
        var originatorId = jwt.Claims
            .Where(c => c.Type == "OriginatorIdentifier")
            .Select(c => Guid.Parse(c.Value))
            .First();
        RefreshToken refreshTokenData;
        try
        {
            refreshTokenData = await _accountService.GetRefreshToken(refreshTokenId);
        }
        catch (ObjectNotFoundException)
        {
            await _accountService.InvalidateRefreshTokenFamily(originatorId);
            throw new InvalidRefreshTokenException($"Refresh token \"{refreshToken}\" was invalid");
        }
        if (refreshTokenData.Expires < DateTime.Now)
        {
            await _accountService.InvalidateRefreshTokenFamily(originatorId);
            throw new InvalidRefreshTokenException($"Refresh token \"{refreshToken}\" was invalid");
        }
        var newRefreshToken = await _accountService.DescendRefreshToken(refreshTokenData, DateTime.Now.AddMonths(1));
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, newRefreshToken.UserId),
        };
        var token = _jwtService.GenerateToken(claims, DateTime.Now.AddDays(14));
        var refresh_claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, newRefreshToken.Id.ToString()),
            new Claim("OriginatorIdentifier", newRefreshToken.Originator.ToString()),
        };
        var refreshTokenJwt = _jwtService.GenerateToken(refresh_claims, newRefreshToken.Expires);
        return new TokenPairDto(token, refreshTokenJwt);
    }

    /// <response code="400">If the refresh token is invalid (invalid_refresh_token)</response>
    [HttpPost("refresh-token/cookie")]
    public async Task<ActionResult<JwtTokenDto>> RefreshTokenCookie(string refreshToken)
    {
        if (!_jwtService.IsValidToken(refreshToken))
            throw new InvalidRefreshTokenException($"Refresh token \"{refreshToken}\" was invalid");
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(refreshToken);
        var refreshTokenId = jwt.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => Guid.Parse(c.Value))
            .First();
        var originatorId = jwt.Claims
            .Where(c => c.Type == "OriginatorIdentifier")
            .Select(c => Guid.Parse(c.Value))
            .First();
        RefreshToken refreshTokenData;
        try
        {
            refreshTokenData = await _accountService.GetRefreshToken(refreshTokenId);
        }
        catch (ObjectNotFoundException)
        {
            await _accountService.InvalidateRefreshTokenFamily(originatorId);
            throw new InvalidRefreshTokenException($"Refresh token \"{refreshToken}\" was invalid");
        }
        if (refreshTokenData.Expires < DateTime.Now)
        {
            await _accountService.InvalidateRefreshTokenFamily(originatorId);
            throw new InvalidRefreshTokenException($"Refresh token \"{refreshToken}\" was invalid");
        }
        var newRefreshToken = await _accountService.DescendRefreshToken(refreshTokenData, DateTime.Now.AddMonths(1));
        var refresh_claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, newRefreshToken.Id.ToString()),
            new Claim("OriginatorIdentifier", newRefreshToken.Originator.ToString()),
        };
        var refreshTokenJwt = _jwtService.GenerateToken(refresh_claims, newRefreshToken.Expires);
        await _accountService.RefreshCookie(newRefreshToken.UserId);
        return new JwtTokenDto(refreshTokenJwt);
    }

    /// <response code="400">If the refresh token is invalid (invalid_refresh_token)</response>
    [HttpPost("invalidate-refresh-token-family")]
    public async Task<ActionResult> InvalidateRefreshTokenFamily(string refreshToken)
    {
        if (!_jwtService.IsValidToken(refreshToken))
            throw new InvalidRefreshTokenException($"Refresh token \"{refreshToken}\" was invalid");
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(refreshToken);
        var originatorId = jwt.Claims
            .Where(c => c.Type == "OriginatorIdentifier")
            .Select(c => Guid.Parse(c.Value))
            .First();
        await _accountService.InvalidateRefreshTokenFamily(originatorId);
        return Ok();
    }

    /// <response code="400">If the login is invalid (invalid_login)</response>
    [HttpPost("login/phone/initialize")]
    [HttpPost("signup/phone/initialize")]
    public async Task<ActionResult> InitializeLoginWithPhone(InitializePhoneLoginDto dto)
    {
        await _accountService.InitializeLoginWithPhone(dto.Phone);
        return Ok();
    }

    /// <response code="400">If the login is invalid (invalid_login)</response>
    [HttpPost("login/phone")]
    [HttpPost("signup/phone")]
    public async Task<ActionResult<UserDto>> LoginWithPhone(LoginWithPhoneDto dto)
    {
        var (result, user) = await _accountService.LoginWithPhone(dto.Phone, dto.VerificationCode, dto.StaySignedIn);
        if (result.Succeeded && user != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user!.Id),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            var refreshToken = await _accountService.GenerateRefreshToken(user.Id, DateTime.Now.AddMonths(1));
            var refresh_claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, refreshToken.Id.ToString()),
                new Claim("OriginatorIdentifier", refreshToken.Originator.ToString()),
            };
            var refreshTokenJwt = _jwtService.GenerateToken(refresh_claims, refreshToken.Expires);
            user.RefreshToken = new JwtTokenDto(refreshTokenJwt);
            return user;
        }
        return Problem(
            statusCode: 400,
            type: "invalid_login",
            title: result.ToString()
        );
    }

    /// <response code="400">If the login is invalid (invalid_login)</response>
    [HttpPost("login/email")]
    public async Task<ActionResult<UserDto>> LoginWithEmail(LoginWithEmailDto dto)
    {
        var (result, user) = await _accountService.LoginWithEmail(dto.Email, dto.Password, dto.StaySignedIn);
        if (result.Succeeded && user != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            var refreshToken = await _accountService.GenerateRefreshToken(user.Id, DateTime.Now.AddMonths(1));
            var refresh_claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, refreshToken.Id.ToString()),
                new Claim("OriginatorIdentifier", refreshToken.Originator.ToString()),
            };
            var refreshTokenJwt = _jwtService.GenerateToken(refresh_claims, refreshToken.Expires);
            user.RefreshToken = new JwtTokenDto(refreshTokenJwt);
            return user;
        }

        return Problem(
          statusCode: 400,
          type: "invalid_login",
          title: result.ToString()
        );
    }

    /// <response code="403">Email is changed to the one already set (constraint_violation)</response>
    /// <response code="400">Email is already confirmed (email_already_confirmed)</response>
    /// <response code="400">Other error setting email. See possible identity errors</response>
    [Authorize]
    [HttpPut("update-email")]
    public async Task<IActionResult> UpdateAccountEmail(UpdateAccountEmailDto dto)
    {
        // User should never be null as the authorize attribute is set
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _accountService.UpdateAccountEmail(userId, dto);

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

    [Authorize]
    [HttpPut("account-details")]
    public async Task<ActionResult<UserDto>> SetAccountDetails(AccountDetailsUpdateDto dto)
    {
        // User should never be null as the authorize attribute is set
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await _accountService.SetAccountDetails(userId, dto);
    }

    /// <response code="400">If the email confirmation failed</response>
    [HttpPost("confirm-email/{email}")]
    public async Task<IActionResult> ConfirmEmail(string email, [FromBody] string token)
    {
        var result = await _accountService.ConfirmEmail(email, token);

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
    /// Sends a new email confirmation email.
    /// </summary>
    [Authorize]
    [HttpPost("send-email-confirmation/{userId}")]
    public async Task<IActionResult> SendConfirmEmail(string userId)
    {
        await _accountService.RequestEmailConfirmationEmail(userId);
        return Ok();
    }

    /// <summary>
    /// Note: Also signs the user in
    /// </summary>
    /// <response code="400">If the email confirmation failed</response>
    /// <response code="404">If no user with the given id was found</response>
    [HttpPost("activate")]
    public async Task<ActionResult<UserDto>> ActivateAccount(ActivateAccountDto dto)
    {
        var (result, user) = await _accountService.ActivateAccount(dto.Email, dto.Password, dto.Token);

        // If the password reset succeeded we sign in the user
        if (result.Succeeded)
        {
            await _accountService.LoginWithoutPassword(user, dto.StaySignedIn);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return await _userService.GetUserDtoById(user.Id);
        }

        return Problem(
            statusCode: 400,
            type: result.Errors.Select(x => x.Code).ToCommaSeparatedString(),
            detail: result.Errors.Select(x => x.Description).ToCommaSeparatedString()
        );
    }

    /// <summary>
    /// Used for requesting an email with a password reset link
    /// </summary>
    /// <response code="404">If no user with the given email was found</response>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
    {
        await _accountService.RequestPasswordReset(forgotPasswordDto);
        return Ok();
    }

    /// <summary>
    /// Note: Also signs the user in if the password reset succeeded
    /// </summary>
    /// <response code="404">If no user with the given email was found</response>
    /// <response code="400">If the password reset failed (most likely a non-valid token)</response>
    [HttpPost("reset-password")]
    public async Task<ActionResult<UserDto>> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        var (result, user) = await _accountService.ResetPassword(resetPasswordDto.Email, resetPasswordDto.Token, resetPasswordDto.Password);

        // If the password reset succeeded we sign in the user
        if (result.Succeeded)
        {
            await _accountService.LoginWithoutPassword(user, resetPasswordDto.StaySignedIn);
            return await _userService.GetUserDtoById(user.Id);
        }

        return Problem(
            statusCode: 400,
            type: result.Errors.Select(x => x.Code).ToCommaSeparatedString(),
            detail: result.Errors.Select(x => x.Description).ToCommaSeparatedString()
        );
    }

    /// <response code="400">If any active marketplace listings exist on the account (ticket_in_marketplace_listing)</response>
    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _accountService.DeleteUser(userId);

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
