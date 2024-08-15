using AutoFixture;
using AutoFixture.Kernel;
using Celebratix.Common.Models.DbModels;

namespace Celebratix.Tests.SpecimenBuilders.Entity;

internal sealed class ChannelEventSpecimenBuilder : AbstractSpecimenBuilder<ChannelEvent>
{
    protected override ChannelEvent CreateDefault(ISpecimenContext context)
    {
        var createDate = context.Create<DateTimeOffset>();

        return new ChannelEvent
        {
            ResaleDisabledDescription = context.Create<string>(),
            ResaleEnabled = context.Create<bool>(),
            ResaleRedirectUrl = context.Create<Uri>(),
            CreatedAt = createDate,
            UpdatedAt = createDate
        };
    }
}