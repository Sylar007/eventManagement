using AutoFixture;
using AutoFixture.Kernel;
using Celebratix.Common.Models.DbModels;

namespace Celebratix.Tests.SpecimenBuilders.Entity;

internal sealed class CategorySpecimenBuilder : AbstractSpecimenBuilder<Category>
{
    protected override Category CreateDefault(ISpecimenContext context) =>
        new()
        {
            Name = context.Create<string>()
        };
}