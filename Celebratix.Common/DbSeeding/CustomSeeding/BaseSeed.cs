using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Microsoft.AspNetCore.Identity;

namespace Celebratix.Common.DbSeeding.CustomSeeding;

public class BaseSeed
{
    #region User constants

    // Password is meant to be changed on the live environment manually.
    // Therefore the lack of a serious password here or fetching of password from appsettings
    private const string AdminUserEmail = "admin@ubit.nu";
    private const string AdminUserPassword = "Password123!";
    private const string AdminUserGivenName = "Admin";
    private const string AdminUserFamilyName = "Adminsson";

    #endregion

    protected readonly CelebratixDbContext DbContext;
    protected readonly UserManager<ApplicationUser> UserManager;

    protected ApplicationUser AdminUser = null!;

    public BaseSeed(CelebratixDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        DbContext = dbContext;
        UserManager = userManager;
    }

    public virtual void Seed(bool save = true)
    {
        // Logic to check if this seed shouldn't be run
        // Could probably be made more advanced (check each role, user etc. to not have to drop the database)
        if (DbContext.Roles.Any())
        {
            return;
        }

        AddRoles();
        AddUsers();

        if (save)
            DbContext.SaveChanges();
    }

    private void AddRoles()
    {
        var superAdminRole = new IdentityRole
        {
            Name = Enums.SuperAdmin,
            NormalizedName = UserManager.NormalizeName(Enums.SuperAdmin)
        };
        var businessRole = new IdentityRole
        {
            Name = Enums.Business,
            NormalizedName = UserManager.NormalizeName(Enums.Business)
        };
        var ticketCheckerRole = new IdentityRole
        {
            Name = Enums.TicketChecker,
            NormalizedName = UserManager.NormalizeName(Enums.TicketChecker)
        };

        DbContext.Roles.AddRange(superAdminRole, businessRole, ticketCheckerRole);
    }

    private void AddUsers()
    {
        AdminUser = new ApplicationUser
        {
            UserName = AdminUserEmail,
            Email = AdminUserEmail,
            EmailConfirmed = true,
            FirstName = AdminUserGivenName,
            LastName = AdminUserFamilyName,
            CreatedAt = DateTimeOffset.UtcNow
        };

        UserManager.CreateAsync(AdminUser, AdminUserPassword).Wait();
        UserManager.AddToRoleAsync(AdminUser, Enums.SuperAdmin).Wait();
    }
}
