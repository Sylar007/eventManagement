using Celebratix.Common.Models.DbModels;

namespace Celebratix.Tests.Extensions;

public static class EventExtensions
{
    public static Event SetBusiness(this Event @event,
        Guid? businessId)
    {
        ArgumentNullException.ThrowIfNull(businessId);

        @event.BusinessId = businessId.Value;

        return @event;
    }

    public static Event SetCategory(this Event @event,
        Category category)
    {
        @event.CategoryId = category.Id;

        return @event;
    }

    public static Event SetCreator(this Event @event,
        ApplicationUser user)
    {
        @event.CreatorId = user.Id;

        return @event;
    }
}