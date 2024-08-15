using AutoFixture;
using AutoFixture.Kernel;
using Celebratix.Common.Models.DbModels;

namespace Celebratix.Tests.SpecimenBuilders.Entity;

internal sealed class ApplicationUserSpecimenBuilder : AbstractSpecimenBuilder<ApplicationUser>
{
    protected override ApplicationUser CreateDefault(ISpecimenContext context)
    {
        var createDate = context.Create<DateTimeOffset>();

        return new ApplicationUser
        {
            Id = context.Create<string>(),
            CreatedAt = createDate,
            Business = context.Create<Business>(),
            Email = context.Create<string>(),
            EmailConfirmed = true
        };
    }
}