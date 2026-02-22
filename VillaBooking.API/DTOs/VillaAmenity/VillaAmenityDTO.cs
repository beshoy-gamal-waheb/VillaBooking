
namespace VillaBooking.API.DTOs.VillaAmenity
{
    public class VillaAmenityDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int VillaId { get; set; }
        public string VillaName { get; set; } = string.Empty;
    }
}
