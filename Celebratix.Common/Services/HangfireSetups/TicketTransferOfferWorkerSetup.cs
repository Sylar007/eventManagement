using Hangfire;

namespace Celebratix.Common.Services.HangfireSetups;

public class TicketTransferWorkerSetup
{
    private const string TicketTransferWorkerId = "TicketTransferWorkerId";

    public static void Setup()
    {
        RecurringJob.AddOrUpdate<TicketTransferService>(TicketTransferWorkerId, x => x.CancelOffersForPastEvents(), Cron.Hourly());
    }
}
