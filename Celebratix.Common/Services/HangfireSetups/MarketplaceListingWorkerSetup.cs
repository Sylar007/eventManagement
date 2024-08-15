using Hangfire;

namespace Celebratix.Common.Services.HangfireSetups;

public class MarketplaceListingWorkerSetup
{
    private const string MarketplaceListingWorkerId = "MarketplaceListingWorkerId";

    public static void Setup()
    {
        RecurringJob.AddOrUpdate<MarketplaceService>(MarketplaceListingWorkerId, x => x.CancelListingsForPastEvents(), Cron.Hourly());
    }
}
