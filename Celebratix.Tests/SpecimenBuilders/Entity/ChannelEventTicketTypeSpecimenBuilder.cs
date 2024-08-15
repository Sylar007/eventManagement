using AutoFixture;
using AutoFixture.Kernel;
using Celebratix.Common.Models.DbModels;

namespace Celebratix.Tests.SpecimenBuilders.Entity;

internal sealed class ChannelEventTicketTypeSpecimenBuilder : AbstractSpecimenBuilder<ChannelEventTicketType>
{
    protected override ChannelEventTicketType CreateDefault(ISpecimenContext context)
    {
        var createDate = context.Create<DateTimeOffset>();

        return new ChannelEventTicketType
        {
            CreatedAt = createDate,
            UpdatedAt = createDate
        };
    }
}