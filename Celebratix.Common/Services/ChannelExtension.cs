using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Services;

public static class ChannelExtension
{
    public static Channel Update(this Channel channel, Action action, bool predicate)
    {
        if (predicate)
        {
            action.Invoke();
        }

        return channel;
    }
}