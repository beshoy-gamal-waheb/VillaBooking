using System.Text;

namespace VillaBooking.Web.Models
{
    public class VillaQueryParameters
    {
        // Filtering
        public string? Name { get; set; }
        public string? Details { get; set; }
        public int? MinOccupancy { get; set; }
        public int? MaxOccupancy { get; set; }
        public double? MinRate { get; set; }
        public double? MaxRate { get; set; }
        public int? MinSqft { get; set; }
        public int? MaxSqft { get; set; }

        // Sorting
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 6;

        public string ToQueryString()
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(Name))
                parts.Add($"name={Uri.EscapeDataString(Name.Trim())}");

            if (!string.IsNullOrWhiteSpace(Details))
                parts.Add($"details={Uri.EscapeDataString(Details.Trim())}");

            if (MinOccupancy.HasValue)
                parts.Add($"minOccupancy={MinOccupancy.Value}");

            if (MaxOccupancy.HasValue)
                parts.Add($"maxOccupancy={MaxOccupancy.Value}");

            if (MinRate.HasValue)
                parts.Add($"minRate={MinRate.Value}");

            if (MaxRate.HasValue)
                parts.Add($"maxRate={MaxRate.Value}");

            if (MinSqft.HasValue)
                parts.Add($"minSqft={MinSqft.Value}");

            if (MaxSqft.HasValue)
                parts.Add($"maxSqft={MaxSqft.Value}");

            if (!string.IsNullOrWhiteSpace(SortBy))
                parts.Add($"sortBy={Uri.EscapeDataString(SortBy.Trim())}");

            if (!string.IsNullOrWhiteSpace(SortOrder))
                parts.Add($"sortOrder={Uri.EscapeDataString(SortOrder.Trim())}");

            parts.Add($"page={Page}");
            parts.Add($"pageSize={PageSize}");

            return parts.Count > 0 ? "?" + string.Join("&", parts) : string.Empty;
        }
    }
}
