using AutoFixture;
using AutoFixture.Kernel;
using Celebratix.Common.Models.DbModels;

namespace Celebratix.Tests.SpecimenBuilders.Entity;

internal sealed class CurrencySpecimenBuilder : AbstractSpecimenBuilder<Currency>
{
    protected override Currency CreateDefault(ISpecimenContext context)
    {
        var createDate = context.Create<DateTimeOffset>();

        return new Currency
        {
            Name = context.Create<string>().Substring(5, 5),
            Code = context.Create<string>().Substring(5, 5),
            CreatedAt = createDate,
            UpdatedAt = createDate,
            Enabled = true,
            DecimalPlaces = 2,
            MinMarketplaceListingPrice = context.Create<decimal>(),
            Symbol = "EUR"
        };
    }
}