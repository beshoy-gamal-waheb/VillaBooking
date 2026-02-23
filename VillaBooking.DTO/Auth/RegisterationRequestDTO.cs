using System.ComponentModel.DataAnnotations;

namespace VillaBooking.DTO.Auth
{
    public class RegisterationRequestDTO
    {

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Role { get; set; } = "Customer";
    }
}
