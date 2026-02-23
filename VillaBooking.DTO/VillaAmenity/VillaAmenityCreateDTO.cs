using System.ComponentModel.DataAnnotations;

namespace VillaBooking.DTO.VillaAmenity
{
    public class VillaAmenityCreateDTO
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public int VillaId { get; set; }
    }
}
