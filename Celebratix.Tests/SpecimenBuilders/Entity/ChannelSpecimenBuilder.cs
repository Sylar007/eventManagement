using AutoFixture;
using AutoFixture.Kernel;
using Celebratix.Common.Models.DbModels;

namespace Celebratix.Tests.SpecimenBuilders.Entity;

internal sealed class ChannelSpecimenBuilder : AbstractSpecimenBuilder<Channel>
{
    protected override Channel CreateDefault(ISpecimenContext context)
    {
        var createDate = context.Create<DateTimeOffset>();

        return new Channel
        {
            Name = context.Create<string>(),
            Color = "000000",
            Slug = context.Create<string>(),
            CreatedAt = createDate,
            UpdatedAt = createDate
        };
    }
}