using System.ComponentModel.DataAnnotations;

namespace VillaBooking.API.DTOs.VillaAmenity
{
    public class VillaAmenityUpdateDTO
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
