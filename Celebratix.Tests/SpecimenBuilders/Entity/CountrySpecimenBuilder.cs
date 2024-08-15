using AutoFixture;
using AutoFixture.Kernel;
using Celebratix.Common.Models.DbModels;

namespace Celebratix.Tests.SpecimenBuilders.Entity;

internal sealed class CountrySpecimenBuilder : AbstractSpecimenBuilder<Country>
{
    protected override Country CreateDefault(ISpecimenContext context)
    {
        var createDate = context.Create<DateTimeOffset>();

        return new Country
        {
            Id = context.Create<string>(),
            Name = context.Create<string>(),
            Enabled = context.Create<bool>(),
            CallingCode = context.Create<string>(),
            ISO3 = context.Create<string>().Substring(5, 3),
        };
    }
}