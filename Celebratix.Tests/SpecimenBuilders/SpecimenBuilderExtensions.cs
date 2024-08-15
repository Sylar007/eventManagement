using AutoFixture;
using Celebratix.Tests.SpecimenBuilders.Entity;

namespace Celebratix.Tests.SpecimenBuilders
{
    internal static class SpecimenBuilderExtensions
    {
        public static IFixture AddEntitySpecimenBuilders(this IFixture fixture)
        {
            fixture.Customizations.Add(new EventSpecimenBuilder());
            fixture.Customizations.Add(new CurrencySpecimenBuilder());
            fixture.Customizations.Add(new ChannelSpecimenBuilder());
            fixture.Customizations.Add(new CategorySpecimenBuilder());
            fixture.Customizations.Add(new CountrySpecimenBuilder());
            fixture.Customizations.Add(new ChannelEventTicketTypeSpecimenBuilder());
            fixture.Customizations.Add(new EventTicketTypeSpecimenBuilder());
            fixture.Customizations.Add(new ChannelEventSpecimenBuilder());
            fixture.Customizations.Add(new BusinessSpecimenBuilder());
            fixture.Customizations.Add(new ApplicationUserSpecimenBuilder());

            return fixture;
        }
    }
}
