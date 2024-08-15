using Hangfire;

namespace Celebratix.Common.Services.HangfireSetups;

public class OrderWorkerSetup
{
    private const string OrderWorkerId = "OrderWorkerId";

    public static void Setup()
    {
        RecurringJob.AddOrUpdate<OrderService>(OrderWorkerId, x => x.CancelExpiredOrders(), Cron.Minutely());
    }
}
