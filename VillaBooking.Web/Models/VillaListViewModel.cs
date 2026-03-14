using VillaBooking.DTO.Villa;

namespace VillaBooking.Web.Models
{
    public class VillaListViewModel
    {
        public List<VillaDTO> Villas { get; set; } = new();
        public VillaQueryParameters Query { get; set; } = new();

        // Pagination metadata (parsed from API response)
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalCount { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
