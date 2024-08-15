using Celebratix.Common.ErrorHandling;
using Celebratix.Common.Exceptions;
using Celebratix.Common.Extensions;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Admin.Users;
using Celebratix.Common.Models.DTOs.Business.Users;
using Celebratix.Common.Models.DTOs.User;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Errors.Model;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace Celebratix.Common.Services;

/// <summary>
/// For interacting with other user accounts in general (not necessarily the own account)
/// </summary>
public class UserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly BusinessService _businessService;
    private readonly CelebratixDbContext _dbContext;
    private readonly ILogger<UserService> _logger;

    public UserService(UserManager<ApplicationUser> userManager, CelebratixDbContext dbContext,
        ILogger<UserService> logger, BusinessService businessService)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _logger = logger;
        _businessService = businessService;
    }

    #region DTO methods

    public async Task<PagedResultDto<UserBasicAdminDto>> GetAllUsersAsAdminDtos(int page, int pageSize)
    {
        return await _userManager.Users
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserBasicAdminDto(u))
            .ToPagedResult(page, pageSize);
    }

    public async Task<List<UserBasicAdminDto>> GetAllAdminUsersAsAdminDtos()
    {
        return await _userManager.Users
            .Where(u => u.Roles!.Any(r => r.Name == Enums.SuperAdmin))
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserBasicAdminDto(u))
            .ToListAsync();
    }

    public async Task<UserDetailedAdminDto> GetUserAsDetailedAdminDto(string userId)
    {
        var user = await _dbContext.Users
            .FirstOrThrowAsync(u => u.Id == userId);

        return new UserDetailedAdminDto(user, await _userManager.GetRolesAsync(user));
    }

    public async Task<List<UserBasicBusinessDto>> GetAllUsersAsBusinessDtos(Guid businessId)
    {
        return await _userManager.Users
            .Where(u => u.BusinessId == businessId)
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserBasicBusinessDto(u))
            .ToListAsync();
    }

    public async Task<UserDetailedBusinessDto> GetUserAsDetailedBusinessDto(Guid businessId, string userId)
    {
        var user = await _dbContext.Users
            .Where(u => u.BusinessId == businessId)
            .FirstOrThrowAsync(u => u.Id == userId);

        return new UserDetailedBusinessDto(user, await _userManager.GetRolesAsync(user));
    }

    private async Task<UserDetailedBusinessDto> GetUserAsDetailedBusinessDto(string userId)
    {
        var user = await _dbContext.Users
            .FirstOrThrowAsync(u => u.Id == userId);

        return new UserDetailedBusinessDto(user, await _userManager.GetRolesAsync(user));
    }

    public async Task<UserDto> GetUserDto(ApplicationUser user)
    {
        _ = user ?? throw new ArgumentNullException(nameof(user));
        return new UserDto(user, await _userManager.GetRolesAsync(user));
    }

    public async Task<UserDto> GetUserDtoById(string userId, [Optional] bool? includePayoutAccounts)
    {
        ApplicationUser? user;
        var users = _dbContext.Users;
        if (includePayoutAccounts.Equals(true))
        {
            user = await users
                .Include(u => u.PayoutAccounts)
                .FirstOrThrowAsync(u => u.Id == userId);
        }
        else
        {
            user = await users.FirstOrThrowAsync(u => u.Id == userId);
        }

        return await GetUserDto(user);
    }

    #endregion

    public async Task VerifyUserHasBusiness(string userId, Guid companyId)
    {
        var hasAccess = await _dbContext.Users
            .Where(u => u.BusinessId == companyId)
            .AnyAsync(u => u.Id == userId);

        if (!hasAccess)
        {
            throw new ForbiddenException("User does not have permission to access the company.");
        }
    }

    public async Task<Result> VerifyUserHasEventAccessV2(string userId, int eventId)
    {
        var businessId = await _dbContext.Events
            .Where(e => e.Id == eventId)
            .Select(e => e.BusinessId)
            .FirstOrThrowAsync();

        var hasAccess = await _dbContext.Users
            .Where(u => u.BusinessId == businessId)
            .AnyAsync(u => u.Id == userId);

        if (!hasAccess)
        {
            return Result.Fail(new CelebratixError(ErrorCode.CelebratixAccessDenied, "User does not have permission to access the event."));
        }

        return Result.Ok();
    }

    public async Task VerifyUserHasEventAccess(string userId, int eventId)
    {
        var businessId = await _dbContext.Events
            .Where(e => e.Id == eventId)
            .Select(e => e.BusinessId)
            .FirstOrThrowAsync();

        var hasAccess = await _dbContext.Users
            .Where(u => u.BusinessId == businessId)
            .AnyAsync(u => u.Id == userId);

        if (!hasAccess)
        {
            throw new ForbiddenException("User does not have permission to access the event.");
        }
    }

    public async Task<Result> VerifyUserHasChannelAccessV2(string userId, Guid channelId)
    {
        var user = await _dbContext.Users
            .Include(u => u.Business)
            .FirstOrThrowAsync(u => u.Id == userId);
        if (user.Business == null)
            return Result.Fail(new CelebratixError(ErrorCode.CelebratixAccessDenied, $"User with id {userId} does not have a business"));

        var channel = await _dbContext.Channels
            .Include(c => c.Business)
            .FirstOrDefaultAsync(c => c.Id == channelId);
        if (channel == null)
            return Result.Fail(new CelebratixError(ErrorCode.CelebratixAccessDenied, $"Channel with id {channelId} does not exist"));

        if (channel.BusinessId != user.Business.Id)
            return Result.Fail(new CelebratixError(ErrorCode.CelebratixAccessDenied, $"User with id {userId}, who is part of business {user.Business.Id} does not have access to channel with id {channelId}"));

        return Result.Ok();
    }

    public async Task VerifyUserHasChannelAccess(string userId, Guid channelId)
    {
        var user = await _dbContext.Users
            .Include(u => u.Business)
            .FirstOrThrowAsync(u => u.Id == userId);
        if (user.Business == null)
            throw new ObjectNotFoundException($"User with id {userId} does not have a business");

        var channel = await _dbContext.Channels
            .Include(c => c.Business)
            .FirstOrDefaultAsync(c => c.Id == channelId);
        if (channel == null)
            throw new ObjectNotFoundException($"Channel with id {channelId} does not exist");

        if (channel.BusinessId != user.Business.Id)
            throw new ForbiddenException($"User with id {userId}, who is part of business {user.Business.Id} does not have access to channel with id {channelId}");
    }

    public async Task VerifyUserHasOrderAccess(string userId, Guid orderId)
    {
        var businessId = await _dbContext.Orders
            .Where(o => o.Id == orderId)
            .Select(o => o.TicketType!.Event!.BusinessId)
            .FirstOrThrowAsync();

        var hasAccess = await _dbContext.Users
            .Where(u => u.BusinessId == businessId)
            .AnyAsync(u => u.Id == userId);

        if (!hasAccess)
        {
            throw new ForbiddenException("User does not have permission to access the order.");
        }
    }

    /// <summary>
    /// Returns the company id
    /// </summary>
    public async Task<Guid> GetUserBusinessId(string userId)
    {
        var companyId = await _dbContext.Users
            .Where(u => u.Id == userId)
            .Where(u => u.BusinessId != null)
            .Select(u => u.BusinessId)
            .FirstOrThrowAsync();
        return companyId!.Value;
    }

    /// <summary>
    /// Returns the company id
    /// </summary>
    public async Task<Result<Guid>> GetUserBusinessIdByClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var businessId = await _dbContext.Users
            .Where(u => u.Id == userId)
            .Where(u => u.BusinessId != null)
            .Select(u => u.BusinessId)
            .FirstOrDefaultAsync();

        if (businessId == Guid.Empty)
        {
            return Result.Fail(new CelebratixError(ErrorCode.CelebratixAccessDenied, "User does not have permission to access the event."));
        }
        return businessId;
    }

    public async Task<ApplicationUser> GetBusinessUser(string userId)
    {
        var user = await _dbContext.Users
            .Where(u => u.Id == userId)
            .Where(u => u.BusinessId != null)
            .Include(u => u.Business)
            .FirstOrThrowAsync();

        return user;
    }

    public async Task<IdentityResult> AddAdminRoleToUser(string userId, Enums.Role role)
    {
        if (role is not (Enums.Role.SuperAdmin))
            throw new UnsupportedRoleException("Only admin roles can be added or removed manually");

        return await AddRoleToUser(userId, role);
    }

    public async Task<IdentityResult> RemoveAdminRoleFromUser(string userId, Enums.Role role)
    {
        if (role is not (Enums.Role.SuperAdmin))
            throw new UnsupportedRoleException("Only admin roles can be added or removed manually");

        return await RemoveRoleFromUser(userId, role);
    }

    public async Task<IdentityResult> AddRoleToUser(string userId, Enums.Role role)
    {
        var user = await _userManager.Users.FirstOrThrowAsync(u => u.Id == userId);

        return await AddRoleToUser(user, role);
    }

    public async Task<IdentityResult> RemoveRoleFromUser(string userId, Enums.Role role)
    {
        var user = await _userManager.Users.FirstOrThrowAsync(u => u.Id == userId);

        return await RemoveRoleFromUser(user, role);
    }

    public async Task<IdentityResult> RemoveRoleFromUser(ApplicationUser user, Enums.Role role)
    {
        _ = user ?? throw new ArgumentNullException(nameof(user));

        var result = await _userManager.RemoveFromRoleAsync(user, role.ToString());

        if (result.Succeeded)
        {
            _logger.LogInformation("User was removed from role: {Role}, Email: {UserEmail}", role.ToString(),
                user.Email);
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Code));
            _logger.LogInformation("Failed removing user from role: {Role}, Email: {UserEmail}\n" +
                                   "Error codes: {Errors}", role.ToString(), user.Email, errors);
        }

        return result;
    }

    public async Task<IdentityResult> AddRoleToUser(ApplicationUser user, Enums.Role role)
    {
        _ = user ?? throw new ArgumentNullException(nameof(user));

        var result = await _userManager.AddToRoleAsync(user, role.ToString());

        if (result.Succeeded)
        {
            _logger.LogInformation("User was added to role: {Role}, Email: {UserEmail}", role.ToString(), user.Email);
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Code));
            _logger.LogInformation("Failed adding user to role: {Role}, Email: {UserEmail}\n" +
                                   "Error codes: {Errors}", role.ToString(), user.Email, errors);
        }

        return result;
    }

    /// <summary>
    /// RequireUniqueEmail in IdentityOptions cannot be used as that blocks empty emails
    /// Therefore a unique index is placed and this method should be used to give back better error messages
    /// (or simply to check if the email is used without creating a new user)
    /// </summary>
    public async Task<bool> IsEmailFree(string email)
    {
        var normalizedEmail = _userManager.NormalizeEmail(email);
        var result = await _userManager.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail);
        return !result;
    }

    /// <exception cref="EmailAlreadyInUseException"></exception>
    public async Task VerifyEmailIsFree(string email)
    {
        if (!await IsEmailFree(email))
        {
            throw new EmailAlreadyInUseException("Email is already in use");
        }
    }

    public async Task<UserDetailedAdminDto> CreateAdminUser(CreateAdminUserAdminDto dto, string creatorEmail)
    {
        if (dto.AdminRole is not (Enums.Role.SuperAdmin))
            throw new UnsupportedRoleException("Only admin roles can be added or removed manually");

        var currentTime = DateTimeOffset.UtcNow;

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            CreatedAt = currentTime,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber
        };

        await CreateApplicationUser(user, creatorEmail);
        await AddRoleToUser(user, dto.AdminRole);
        await SendAdminAccountSetupEmail(user);
        return await GetUserAsDetailedAdminDto(user.Id);
    }

    public async Task<UserDetailedAdminDto> CreateBusinessUserAsAdmin(CreateBusinessUserAdminDto dto, string creatorEmail)
    {
        var company = await _businessService.GetBusiness(dto.BusinessId);

        var user = await CreateBusinessUser(dto, company, dto.Role, creatorEmail);
        return await GetUserAsDetailedAdminDto(user.Id);
    }

    public async Task<UserDetailedBusinessDto> CreateBusinessUserAsBusiness(CreateBusinessUserDto dto, string userId)
    {
        var currentUser = await GetBusinessUser(userId);
        var company = currentUser.Business!;

        var user = await CreateBusinessUser(dto, company, dto.Role, currentUser.Email);
        return await GetUserAsDetailedBusinessDto(user.Id);
    }

    private async Task<ApplicationUser> CreateBusinessUser(CreateUserDto createUserDto, Business business, Enums.Role role, string? creatorEmail)
    {
        var currentTime = DateTimeOffset.UtcNow;

        var user = new ApplicationUser
        {
            UserName = createUserDto.Email,
            Email = createUserDto.Email,
            CreatedAt = currentTime,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            PhoneNumber = createUserDto.PhoneNumber,
            BusinessId = business.Id,
        };

        await CreateApplicationUser(user, creatorEmail);
        await AddRoleToUser(user, role);
        await SendCompanyAccountSetupEmail(user, business);
        return user;
    }

    private async Task CreateApplicationUser(ApplicationUser user, string? creatorEmail)
    {
        if (creatorEmail == null)
        {
            throw new ArgumentException("Creator of new accounts must have an email set");
        }

        // Returns proper error message, easier for user to understand the problem in case of error
        if (user.Email != null)
        {
            await VerifyEmailIsFree(user.Email);
        }

        var result = await _userManager.CreateAsync(user);

        if (result.Succeeded)
        {
            _logger.LogInformation("New user created (via admin-panel), Id: {UserId}, Email: {UserEmail}. Created by: {CreatorEmail}",
                user.Id, user.Email, creatorEmail);
            return;
        }
        var errors = string.Join(", ", result.Errors.Select(e => e.Code));
        _logger.LogInformation("User admin creation attempt failed, Email: {UserEmail}. Attempt by: {CreatorEmail}\n" +
                               "Error codes: {Errors}", user.Email, creatorEmail, errors);
        throw new Exception("User creation from admin failed"); // Internal server error
    }

    public async Task SendAccountSetupEmail(string userId)
    {
        var user = await _userManager.Users
            .Include(u => u.Business)
            .FirstOrThrowAsync(u => u.Id == userId);

        if (user.EmailConfirmed)
        {
            throw new AccountAlreadyActivatedException();
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains(Enums.SuperAdmin))
        {
            await SendAdminAccountSetupEmail(user);
        }
        else
        {
            if (user.Business == null)
            {
                throw new ObjectNotFoundException("User has no company set");
            }
            await SendCompanyAccountSetupEmail(user, user.Business);
        }
    }

    /// <summary>
    /// Current user has to be part of the same business
    /// </summary>
    public async Task SendCompanyAccountSetupEmail(string currentUserId, string userToActivateId)
    {
        var currentUser = await _userManager.Users
            .FirstOrThrowAsync(u => u.Id == currentUserId);

        var userToActivate = await _userManager.Users
            .Where(u => u.BusinessId == currentUser.BusinessId)
            .Include(u => u.Business)
            .FirstOrThrowAsync(u => u.Id == userToActivateId);

        if (userToActivate.Business == null)
        {
            throw new ObjectNotFoundException("User has no company set");
        }

        if (userToActivate.EmailConfirmed)
        {
            throw new AccountAlreadyActivatedException();
        }

        await SendCompanyAccountSetupEmail(userToActivate, userToActivate.Business);
    }

    private async Task SendAdminAccountSetupEmail(ApplicationUser user)
    {
        var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        _logger.LogInformation("Generated account activation (email confirm.) token, Email: {UserEmail}", user.Email);
        NotificationService.SendAdminAccountSetupEmailWithHangfire(user, confirmEmailToken);
    }

    private async Task SendCompanyAccountSetupEmail(ApplicationUser user, Business business)
    {
        var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        _logger.LogInformation("Generated account activation (email confirm.) token, Email: {UserEmail}", user.Email);
        NotificationService.SendCompanyAccountSetupEmailWithHangfire(user, business, confirmEmailToken);
    }
}
