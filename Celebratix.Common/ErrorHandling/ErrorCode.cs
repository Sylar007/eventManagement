namespace Celebratix.Common.ErrorHandling
{
    public enum ErrorCode
    {
        //Generic error
        Generic = 0,

        //Validation
        GenericValidator = 100,
        NotNullValidator,
        NotEmptyValidator,
        NotEqualValidator,
        NullValidator,
        EqualValidator,
        EmptyValidator,
        EmailValidator,
        LengthValidator,
        GreaterThanValidator,
        GreaterThanOrEqualValidator,
        LessThanValidator,
        LessThanOrEqualValidator,

        //Celebratix
        CelebratixGeneric = 300,
        CelebratixAccessDenied,
        CelebratixChannelSlugAlreadyExists,
        CelebratixEventInvalidOrNotFound,
        CelebratixEventNoTicketTypesFound,
        CelebratixEventNoChannelsFound,
        CelebratixChannelInvalidOrNotFound
    }
}
