using System.ComponentModel.DataAnnotations;

namespace Celebratix.Common.Models.DTOs.User;

public class CreateStripeConnectAccountDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public string AccountNumber { get; set; } = null!;
    public string AddressLine1 { get; set; } = null!;
    public string City { get; set; } = null!;
    public string CountryCode { get; set; } = null!;
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }
    public string PostalCode { get; set; } = null!;
    public string CurrencyCode { get; set; } = "EUR";
}
