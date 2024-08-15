using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.User.Events;

/// <summary>
/// Required includes:
/// Category
/// Image
/// </summary>
public class EventBasicDto
{
    public EventBasicDto(Event dbModel)
    {
        Id = dbModel.Id;
        CustomSlug = dbModel.CustomSlug;
        Visible = dbModel.Visible;
        Name = dbModel.Name;
        AgeLimit = dbModel.AgeLimit;
        StartDate = dbModel.StartDate;
        OpenDate = dbModel.OpenDate;
        EndDate = dbModel.EndDate;
        Location = dbModel.Location;
        City = dbModel.City;
        Website = dbModel.Website;
        Category = dbModel.Category != null ? new CategoryDto(dbModel.Category) : null;
        Image = dbModel.Image != null ? new ImageDto(dbModel.Image) : null;
        ExternalEventUrl = dbModel.ExternalEventUrl;
    }

    public int Id { get; set; }

    /// <summary>
    /// Can be used instead of the id to fetch the event
    /// </summary>
    public string? CustomSlug { get; set; }

    public string Name { get; set; }

    public int? AgeLimit { get; set; }

    public bool Visible { get; set; }

    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset OpenDate { get; set; }

    public DateTimeOffset EndDate { get; set; }

    public CategoryDto? Category { get; set; }

    public string? Location { get; set; }

    public string? Website { get; set; }

    public string? City { get; set; }

    public ImageDto? Image { get; set; }

    public string? ExternalEventUrl { get; set; }
}
