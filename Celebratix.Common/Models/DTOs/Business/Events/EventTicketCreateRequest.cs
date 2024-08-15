using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Celebratix.Common.Models.DTOs.Business.Events
{
    public class EventTicketCreateRequest
    {
        public int EventId { get; set; }

        public string Name { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }

        public int CategoryId { get; set; }

        public decimal Price { get; set; }

        public decimal ServiceFee { get; set; }

        public int Capacity { get; set; }

        public int MaxTicketsPerPurchase { get; set; }

        public int MinTicketsPerPurchase { get; set; }

        public DateTimeOffset AvailableFrom { get; set; }

        public DateTimeOffset AvailableUntil { get; set; }

        public bool HideSoldOut { get; set; }
    }
}
