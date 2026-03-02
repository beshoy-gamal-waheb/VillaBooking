using System.ComponentModel.DataAnnotations;

namespace VillaBooking.DTO.Villa
{
    public class VillaUpsertDTO
    {
        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }

        [MaxLength(200)]
        public string? Details { get; set; }

        [Required]
        [Range(1, 10000, ErrorMessage = "Rate must be between 1 and 10,000")]
        public double Rate { get; set; }

        [Required]
        [Range(100, 10000, ErrorMessage = "Square feet must be between 100 and 10,000")]
        public int Sqft { get; set; }

        [Required]
        [Range(1, 50, ErrorMessage = "Occupancy must be between 1 and 50")]
        public int Occupancy { get; set; }

        [Url(ErrorMessage = "Please enter a valid image URL")]
        public string? ImageUrl { get; set; }
    }
}
