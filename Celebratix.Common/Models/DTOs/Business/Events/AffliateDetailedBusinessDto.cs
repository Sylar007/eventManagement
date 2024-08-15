using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.Business.Events;

public class AffliateDetailedBusinessDto : AffliateBusinessDto
{
    public AffliateDetailedBusinessDto(Affiliate dbModel) : base(dbModel)
    {
    }
}
