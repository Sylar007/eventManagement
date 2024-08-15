using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Celebratix.Common.Models.DbModels;

public class PayoutAccount : DbModelBase
{
    public Guid Id { get; set; }
    [ForeignKey(nameof(Owner))]
    public string? OwnerId { get; set; }
    public ApplicationUser? Owner { get; set; }

    [ForeignKey(nameof(Country))]
    public string? CountryCode { get; set; }
    public Country? Country { get; set; }

    [ForeignKey(nameof(Currency))]
    [MaxLength(5)]
    public string? CurrencyCode { get; set; }
    public Currency? Currency { get; set; }

    [MaxLength(100)]
    [RegularExpression(@"^[^^!#$%*=<>;{}""]*$", ErrorMessage = "First name can not contain any of ^!#$%*=<>;{}\"")]
    public string FirstName { get; set; } = null!;

    [MaxLength(100)]
    [RegularExpression(@"^[^^!#$%*=<>;{}""]*$", ErrorMessage = "Last name can not contain any of ^!#$%*=<>;{}\"")]
    public string LastName { get; set; } = null!;

    [MaxLength(300)]
    public string AccountName { get; set; } = null!;

    [MaxLength(100)]
    public string AccountNumber { get; set; } = null!;

    [MaxLength(1000)]
    public string AddressLine1 { get; set; } = null!;

    [MaxLength(200)]
    public string City { get; set; } = null!;

    [MaxLength(50)]
    public string PostalCode { get; set; } = null!;

    // public Enums.PayoutAccountStatus Status { get; set; }

    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [MaxLength(50)]
    public string IpAddress { get; set; } = null!;
}
