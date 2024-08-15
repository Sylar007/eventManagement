namespace Celebratix.Common.Models.DTOs.Business.Channel;

public sealed class ChannelUpdateResaleRequest
{
    public Guid ChannelId { get; set; }
    public bool Enabled { get; set; }
    public string? DisabledDescription { get; set; }
    public string? RedirectUrl { get; set; }
}