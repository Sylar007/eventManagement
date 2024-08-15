using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.Business.Payouts;

public class BusinessPayoutBusinessDto
{
    public BusinessPayoutBusinessDto(BusinessPayout dbModel)
    {
        Id = dbModel.Id;
        Amount = dbModel.Amount;
        Comment = dbModel.Comment;
    }

    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    public string? Comment { get; set; }
}
