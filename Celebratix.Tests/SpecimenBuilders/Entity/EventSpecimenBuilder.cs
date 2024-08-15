using AutoFixture;
using AutoFixture.Kernel;
using Celebratix.Common.Models.DbModels;

namespace Celebratix.Tests.SpecimenBuilders.Entity;

internal sealed class EventSpecimenBuilder : AbstractSpecimenBuilder<Event>
{
    protected override Event CreateDefault(ISpecimenContext context)
    {
        var startDate = context.Create<DateTimeOffset>();

        return new Event
        {
            Name = context.Create<string>(),
            AddressLine1 = context.Create<string>(),
            AddressLine2 = context.Create<string>(),
            AgeLimit = context.Create<int>(),
            City = context.Create<string>(),
            Code = context.Create<string>(),
            Postcode = context.Create<string>(),
            Description = context.Create<string>(),
            Website = context.Create<string>(),
            Visible = context.Create<bool>(),
            TicketTextColor = "000000",
            TicketBackgroundColor = "000000",
            Location = context.Create<string>(),
            Publish = context.Create<bool>(),
            ExternalEventUrl = context.Create<string>(),
            CustomEmailTemplateId = context.Create<string>(),
            CustomSlug = context.Create<string>(),
            StartDate = startDate,
            EndDate = startDate.AddDays(context.Create<int>()),
            OpenDate = startDate,
            UpdatedAt = startDate,
            CreatedAt = startDate,
            Currency = context.Create<Currency>()
        };
    }
}