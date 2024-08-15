namespace Celebratix.Common.Models.DTOs.User.Orders;

public class OrderCreateResponseDto
{
    public Guid OrderId { get; set; }

    public string PaymentIntentClientSecret { get; set; } = null!;

    public string StripeCustomerId { get; set; } = null!;

    public string EphemeralKeySecret { get; set; } = null!;

    public DateTimeOffset ValidUntil { get; set; }

    public Enums.OrderStatus OrderStatus { get; set; }
}
