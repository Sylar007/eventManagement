using Microsoft.AspNetCore.Http;

namespace Celebratix.Common.Models.DTOs.Business.Channel
{
    public class ChannelRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public Enums.ChannelTemplateTypes TemplateType { get; set; }
        public string? Color { get; set; }
        public IFormFile? Image { get; set; }
    }
}
