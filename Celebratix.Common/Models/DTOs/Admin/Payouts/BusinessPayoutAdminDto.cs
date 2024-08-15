using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.Admin.Payouts;

public class BusinessPayoutAdminDto
{
    public BusinessPayoutAdminDto(BusinessPayout dbModel)
    {
        Id = dbModel.Id;
        Amount = dbModel.Amount;
        Comment = dbModel.Comment;
    }

    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    public string? Comment { get; set; }
}
