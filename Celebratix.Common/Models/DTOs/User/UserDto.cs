using System.ComponentModel.DataAnnotations;
using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.User;

public class UserDto
{
    /// <summary>
    /// Required includes:
    /// KycApplication
    /// </summary>
    public UserDto(ApplicationUser user, ICollection<string> roles)
    {
        Id = user.Id;
        Roles = roles;
        PayoutAccounts = user.PayoutAccounts?
            .Select(pa => new PayoutAccountDto(pa))
            .ToList();
        Email = user.Email;
        FirstName = user.FirstName;
        LastName = user.LastName;
        FullName = user.FullName;
        PhoneNumber = user.PhoneNumber;
        EmailConfirmed = user.EmailConfirmed;
        Gender = user.Gender;
        DateOfBirth = user.DateOfBirth;
        StripePayoutRequirementsSubmitted = user.StripePayoutRequirementsSubmitted;
        StripePayoutRequirementsFulfilled = user.StripePayoutRequirementsFulfilled;
    }

    public string Id { get; set; }

    public ICollection<string> Roles { get; set; }

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public bool EmailConfirmed { get; set; }

    public Enums.Gender? Gender { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    public JwtTokenDto? Token { get; set; }

    public JwtTokenDto? RefreshToken { get; set; }

    public ICollection<PayoutAccountDto>? PayoutAccounts { get; set; }

    public bool StripePayoutRequirementsSubmitted { get; set; }

    public bool StripePayoutRequirementsFulfilled { get; set; }
}
