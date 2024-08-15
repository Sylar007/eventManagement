using Celebratix.Common.SwaggerFilters;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Celebratix.Common.Models.DTOs.Business.Channel;

public class ChannelCreateBusinessRequest
{
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [MaxLength(50)]
    [SwaggerOptional]
    public string? Slug { get; set; }
    public int? Layout { get; set; }
    public int? ColorMode { get; set; }
    public int? Theme { get; set; }
    public string? Color { get; set; }
    public IFormFile? Image { get; set; }
}