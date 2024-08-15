using Celebratix.Common.Models;

namespace Celebratix.Common.Extensions
{
    public static class EnumsExtensions
    {
        public static Dictionary<Enums.SortDirection, string> SortDirectionDescriptions = new Dictionary<Enums.SortDirection, string>()
        {
            { Enums.SortDirection.Ascending, "Ascending" },

            { Enums.SortDirection.Descending, "Descending" }
        };

        public static string GetSortDirectionDescription(Enums.SortDirection sortDirection)
        {
            if (SortDirectionDescriptions.TryGetValue(sortDirection, out string? description))
            {
                return description;
            }

            // Could't find the mapping value in the dictionary.
            // Return a default string.
            return "Unknown";
        }
    }
}
