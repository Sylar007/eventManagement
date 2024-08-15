using AutoFixture;
using AutoFixture.Kernel;
using Celebratix.Common.Models.DbModels;

namespace Celebratix.Tests.SpecimenBuilders.Entity;

internal sealed class BusinessSpecimenBuilder : AbstractSpecimenBuilder<Business>
{
    protected override Business CreateDefault(ISpecimenContext context)
    {
        var createDate = context.Create<DateTimeOffset>();

        return new Business
        {
            Name = context.Create<string>(),
            ApplicationFee = context.Create<decimal>(),
            Country = context.Create<Country>(),
            EventsPageUrl = context.Create<string>(),
            FacebookPixelId = context.Create<string>(),
            CreatedAt = createDate,
            UpdatedAt = createDate
        };
    }
}