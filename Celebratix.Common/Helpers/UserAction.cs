using Celebratix.Common.SwaggerFilters;

/// <summary>
/// Represents a user action that can be performed on a resource. For example, creating a transfer offer.
/// It is meant to easily represent the 3 usual states that a user action can be in:
/// 1. Thing does not exist and can be created {Data = null, UnavailableReason = null}
/// 2. Thing exists and can be viewed or updated {Data = data, UnavailableReason = null}
/// 3. Thing may exist (but is not passed) and user cannot create a new one {Data = null, UnavailableReason = reason}
/// </summary>
public class UserAction<TData, TReason>
    where TReason : unmanaged
{
    public UserAction(TData? data)
    {
        Data = data;
    }
    public UserAction(TReason? unavailableReason)
    {
        UnavailableReason = unavailableReason;
    }

    public TData? Data { get; }

    [SwaggerOptional]
    public TReason? UnavailableReason { get; }
}
