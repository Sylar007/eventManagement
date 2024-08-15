using Celebratix.Common.Models.DbModels;

namespace Celebratix.Tests.Extensions
{
    public static class ChannelExtensions
    {
        public static Channel SetBusiness(this Channel channel,
            Guid? businessId)
        {
            ArgumentNullException.ThrowIfNull(businessId);

            channel.BusinessId = businessId.Value;

            return channel;
        }

        public static IEnumerable<Channel> SetBusiness(
            this IEnumerable<Channel> channels,
            Guid? businessId)
        {
            ArgumentNullException.ThrowIfNull(businessId);

            return channels
                .Select(channel =>
                    channel.SetBusiness(businessId));
        }
    }
}
