using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.User;

public class PayoutAccountDto
{
    public PayoutAccountDto(PayoutAccount account)
    {
        CountryCode = account.CountryCode;
        CurrencyCode = account.CurrencyCode;
        FirstName = account.FirstName;
        LastName = account.LastName;
        AccountName = account.AccountName;
        var accountNumber = account.AccountNumber;
        var accountNumberLength = accountNumber.Length;
        AccountNumberLast4 = accountNumberLength <= 4 ? accountNumber : accountNumber.Substring(Math.Max(0, accountNumberLength - 4));
        AddressLine1 = account.AddressLine1;
        City = account.City;
        PostalCode = account.PostalCode;
        DateOfBirth = account.DateOfBirth;
    }

    public string? CountryCode { get; set; }

    public string? CurrencyCode { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string AccountName { get; set; }

    public string AccountNumberLast4 { get; set; }

    public string AddressLine1 { get; set; }

    public string City { get; set; }

    public string PostalCode { get; set; }

    public DateTime DateOfBirth { get; set; }
}
