using Amazon.Runtime.Internal;
using Celebratix.Common.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Celebratix.Common.Models.DTOs.Business.Events
{
    public class EventTicketDashboardDto
    {
        public EventTicketDashboardDto(EventTicketType dbModel)
        {
            Id = dbModel.Id;
            Name = dbModel.Name;
            if (dbModel.Image != null && dbModel.TicketImageId != null)
            {
                Image = new ImageDto(dbModel.Image);
            }
            Price = dbModel.Price;
            ServiceFee = dbModel.ServiceFee;
            CategoryId = dbModel.CategoryId;
            AvailableFrom = dbModel.AvailableFrom;
            AvailableUntil = dbModel.AvailableUntil;
            HideSoldOut = dbModel.HideSoldOut;
            MinTicketsPerPurchase = dbModel.MinTicketsPerPurchase;
            MaxTicketsPerPurchase = dbModel.MaxTicketsPerPurchase;
            Capacity = dbModel.MaxTicketsAvailable;
        }
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public ImageDto? Image { get; set; }

        public int? CategoryId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only positive numbers allowed")]
        [Precision(18, 6)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only positive numbers allowed")]
        [Precision(18, 6)]
        public decimal ServiceFee { get; set; }

        public DateTimeOffset? AvailableFrom { get; set; }

        public DateTimeOffset? AvailableUntil { get; set; }

        public int MinTicketsPerPurchase { get; set; }

        public int MaxTicketsPerPurchase { get; set; }

        public int? Capacity { get; set; }

        public bool HideSoldOut { get; set; } = false;
    }
}
