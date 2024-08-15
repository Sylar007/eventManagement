using AutoFixture;
using AutoFixture.Kernel;
using Celebratix.Common.Models.DbModels;

namespace Celebratix.Tests.SpecimenBuilders.Entity;

internal sealed class EventTicketTypeSpecimenBuilder : AbstractSpecimenBuilder<EventTicketType>
{
    protected override EventTicketType CreateDefault(ISpecimenContext context)
    {
        var createDate = context.Create<DateTimeOffset>();

        return new EventTicketType
        {
            Name = context.Create<string>(),
            ApplicationFeeOverwrite = context.Create<decimal>(),
            AvailableFrom = createDate,
            AvailableUntil = createDate.AddDays(context.Create<int>()),
            CustomVat = context.Create<decimal>(),
            HideSoldOut = context.Create<bool>(),
            LinkCode = context.Create<string>(),
            MaxTicketsAvailable = context.Create<int>(),
            MaxTicketsPerPurchase = context.Create<int>() % 1000,
            MinTicketsPerPurchase = context.Create<int>() % 4,
            OnlyAffiliates = context.Create<bool>(),
            Price = context.Create<decimal>(),
            ReservedTickets = context.Create<int>() % 10,
            PubliclyAvailable = context.Create<bool>(),
            ServiceFee = context.Create<decimal>(),
            SortIndex = context.Create<int>(),
            TicketsCheckedIn = context.Create<int>() % 50,
            TicketsSold = context.Create<int>() % 50,
            CreatedAt = createDate,
            UpdatedAt = createDate
        };
    }
}