using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs
{
    public class AmountDto
    {
        public AmountDto()
        {
            Base = decimal.Zero;
            ServiceFee = decimal.Zero;
            ApplicationFee = decimal.Zero;
        }

        public AmountDto(decimal @base, decimal serviceFee, decimal applicationFee)
        {
            Base = @base;
            ServiceFee = serviceFee;
            ApplicationFee = applicationFee;
        }

        public void AddOrder(Order o)
        {
            Base += o.BaseAmount;
            ServiceFee += o.ServiceAmount;
            ApplicationFee += o.ApplicationAmount;
        }

        public decimal Base { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal ApplicationFee { get; set; }
    }
}
