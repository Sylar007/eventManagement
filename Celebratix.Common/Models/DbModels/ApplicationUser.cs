using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

// ReSharper disable CollectionNeverUpdated.Global

namespace Celebratix.Common.Models.DbModels;

[Index(nameof(CreatedAt))]
// The reason this is set here instead of being recorded as an option in the IdentityOptions is because
// then multiple null emails is not allowed
[Index(nameof(NormalizedEmail), IsUnique = true)]
public class ApplicationUser : IdentityUser
{
    public ICollection<IdentityRole>? Roles { get; set; }

    public DateTimeOffset LastLoggedIn { get; set; }

    /// <summary>
    /// I.e. the sign up date & time
    /// </summary>
    [Required]
    public DateTimeOffset CreatedAt { get; set; }

    [RegularExpression(@"^[^^!#$%*=<>;{}""]*$", ErrorMessage = "First name can not contain any of ^!#$%*=<>;{}\"")]
    public string? FirstName { get; set; }

    [RegularExpression(@"^[^^!#$%*=<>;{}""]*$", ErrorMessage = "Last name can not contain any of ^!#$%*=<>;{}\"")]
    public string? LastName { get; set; }

    [NotMapped]
    public string FullName => Strings.Trim($"{FirstName} {LastName}");

    public Enums.Gender? Gender { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    #region Customer

    public string? StripeCustomerId { get; set; }

    public string? StripeConnectAccountId { get; set; }

    public bool StripePayoutRequirementsSubmitted { get; set; } = false;

    public bool StripePayoutRequirementsFulfilled { get; set; } = false;

    public ICollection<Ticket>? Tickets { get; set; }

    public ICollection<Order>? Orders { get; set; }

    [NotMapped]
    public ICollection<Ticket>? UpcomingTickets =>
        Tickets?.Where(t => t.TicketType?.Event?.EndDate >= DateTimeOffset.UtcNow).ToList();

    [NotMapped]
    public ICollection<Ticket>? PastTickets =>
        Tickets?.Where(t => DateTimeOffset.UtcNow >= t.TicketType?.Event?.EndDate).ToList();

    [InverseProperty(nameof(MarketplaceListing.Seller))]
    public ICollection<MarketplaceListing>? MarketplaceListings { get; set; }

    [InverseProperty(nameof(TicketTransferOffer.Transferor))]
    public ICollection<TicketTransferOffer>? TicketTransferRequests { get; set; }

    [InverseProperty(nameof(TicketTransferOffer.Receiver))]
    public ICollection<TicketTransferOffer>? AcceptedTicketTransfers { get; set; }

    [NotMapped]
    public ICollection<MarketplaceListing>? ActiveMarketplaceListings =>
        MarketplaceListings?.Where(ml => !ml.Sold).ToList();

    [NotMapped]
    public ICollection<MarketplaceListing>? SoldMarketplaceListings =>
        MarketplaceListings?.Where(ml => ml.Sold).ToList();

    [InverseProperty(nameof(MarketplaceListing.Buyer))]
    public ICollection<MarketplaceListing>? MarketplacePurchases { get; set; }

    [NotMapped]
    public ICollection<TicketTransferOffer>? ActiveTicketTransferRequests =>
        TicketTransferRequests?.Where(ml => ml.Available).ToList();

    [InverseProperty(nameof(PayoutAccount.Owner))]
    public ICollection<PayoutAccount>? PayoutAccounts { get; set; }

    #endregion

    #region Business

    [ForeignKey(nameof(DbModels.Business))]
    public Guid? BusinessId { get; set; }
    public Business? Business { get; set; }

    #endregion
}
