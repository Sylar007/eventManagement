using System.ComponentModel.DataAnnotations.Schema;

namespace Celebratix.Common.Models.DTOs.Business.Channel
{
    public class ChannelDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Enums.ChannelTemplateTypes TemplateType { get; set; }
        public string? Color { get; set; }

        [ForeignKey(nameof(Image))]
        public Guid? CustomBackground { get; set; }
        public ImageDto? Image { get; set; }
        public ChannelDto(DbModels.Channel dbModel)
        {
            Id = dbModel.Id;
            Name = dbModel.Name;
            TemplateType = dbModel.TemplateType;
            Color = dbModel.Color;
            Image = dbModel.CustomBackground != null ? new ImageDto(dbModel.CustomBackground) : null;
            CustomBackground = dbModel.CustomBackgroundId;
        }
    }
}
