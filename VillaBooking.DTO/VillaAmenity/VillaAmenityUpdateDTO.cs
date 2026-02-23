using System.ComponentModel.DataAnnotations;

namespace VillaBooking.DTO.VillaAmenity
{
    public class VillaAmenityUpdateDTO
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
