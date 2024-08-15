using Celebratix.Common.Services;

namespace Celebratix.Extensions
{
    internal static class ServicesExtensions
    {
        internal static IServiceCollection AddServices(this IServiceCollection services) =>
            services
                .AddScoped(typeof(AccountService))
                .AddScoped(typeof(UserService))
                .AddScoped(typeof(JwtService))
                .AddScoped(typeof(CurrencyService))
                .AddScoped(typeof(CountryService))
                .AddScoped(typeof(BusinessService))
                .AddScoped(typeof(EventService))
                .AddScoped(typeof(OrderService))
                .AddScoped(typeof(OrderAggregatorService))
                .AddScoped(typeof(TicketService))
                .AddScoped(typeof(MagicService))
                .AddScoped(typeof(MarketplaceService))
                .AddScoped(typeof(CategoryService))
                .AddScoped(typeof(AffiliateCodeService))
                .AddScoped(typeof(ChannelService))
                .AddScoped(typeof(OrderService))
                .AddScoped(typeof(PaymentService))
                .AddScoped(typeof(TicketTransferService))
                .AddScoped(typeof(TrackingService))
                .AddScoped(typeof(TransactionService))
                .AddScoped(typeof(BusinessPayoutService))
                .AddScoped(typeof(NotificationService))
                .AddScoped(typeof(ChannelEventTicketTypeService))
                .AddScoped(typeof(ConfigService));
    }
}
