namespace Celebratix.Common.Models.DTOs;

public class PagedResultDto<T> where T : class // if an interface for dtos is added, class should be replaced with that interface
{
    public IEnumerable<T> List { get; set; } = null!;

    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int RowCount { get; set; }
    public int PageCount { get; set; }
}
