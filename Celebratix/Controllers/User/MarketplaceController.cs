using System.Security.Claims;
using Celebratix.Common.Configs;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.User.Marketplace;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Celebratix.Controllers.User;

[Area("user")]
[Route("[area]/[controller]")]
[ApiController]
[Authorize]
public class MarketplaceController : ControllerBase
{
    private readonly MarketplaceService _marketplaceService;
    private readonly MarketplaceConfig _marketplaceConfig;

    public MarketplaceController(MarketplaceService marketplaceService, IOptions<MarketplaceConfig> marketplaceConfig)
    {
        _marketplaceService = marketplaceService;
        _marketplaceConfig = marketplaceConfig.Value;
    }

    /// <summary>
    /// Sorted by price
    /// </summary>
    [AllowAnonymous]
    [HttpGet("{eventId:int}")]
    public async Task<PagedResultDto<MarketplaceListingBasicDto>> GetListingsForEvent(int eventId, Guid? ticketTypeId = null, int page = 1, int pageSize = 20)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var listings = await _marketplaceService.GetListingsForEventAsUserDtos(eventId, ticketTypeId, currentUserId, page, pageSize);
        return listings;
    }

    /// <summary>
    /// Sorted by date
    /// </summary>
    [HttpGet("own")]
    public async Task<PagedResultDto<MarketplaceListingBasicDto>> GetOwnListings(int page = 1, int pageSize = 20, bool? sold = null)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var listings = await _marketplaceService.GetListingsForUserAsDto(currentUserId, page, pageSize, sold);
        return listings;
    }

    [AllowAnonymous]
    [HttpGet("{listingId:guid}")]
    public async Task<ActionResult<MarketplaceListingDetailedDto>> GetListing(Guid listingId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var listing = await _marketplaceService.GetListingAsDetailedUserDto(listingId, currentUserId);
        return listing;
    }

    /// <summary>
    /// Can only be done before payment information has been submitted
    /// </summary>
    /// <response code="400">If the event end time has already passed (event_end_time_passed)</response>
    /// <response code="400">If price isn't valid (argument_out_of_range)</response>
    /// <response code="400">Payouts isn't enabled on the user. Make sure the payouts verification process is completed (payouts_not_enabled_on_user)</response>
    /// <response code="400">Ticket has already been scanned/checked in (ticket_already_scanned)</response>
    /// <response code="400">The ticket is in an active marketplace listing (ticket_in_marketplace_listing)</response>
    /// <response code="400">The ticket is in an active transfer offer (ticket_in_transfer_offer)</response>
    /// <response code="400">The ticket was sold or transferred (ticket_sold_or_transferred)</response>
    /// <response code="400">The event end time has passed (event_end_time_passed)</response>
    [HttpPost]
    public async Task<ActionResult<MarketplaceListingDetailedDto>> CreateListing(MarketplaceListingCreateDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var listing = await _marketplaceService.CreateListing(dto, currentUserId);
        return listing;
    }

    /// <summary>
    /// Can only be done before payment information has been submitted
    /// </summary>
    /// <response code="400">The listing isn't available for being cancelled (listing_not_available)</response>
    [HttpPost("{listingId:guid}/cancel")]
    public async Task<ActionResult<MarketplaceListingDetailedDto>> CancelListing(Guid listingId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var listing = await _marketplaceService.CancelListing(currentUserId, listingId);
        return listing;
    }

    [AllowAnonymous]
    [HttpGet("constants")]
    public ActionResult<MarketplaceConstantsDto> GetMarketplaceConstants()
    {
        return new MarketplaceConstantsDto
        {
            MaxPriceOverOriginal = _marketplaceConfig.MaxPriceOverOriginal,
            SecondaryFeeFraction = _marketplaceConfig.SecondaryFeeFraction,
            ServiceFeeFraction = _marketplaceConfig.ServiceFeeFraction,
            ServiceFeeVat = _marketplaceConfig.ServiceFeeVat
        };
    }
}
