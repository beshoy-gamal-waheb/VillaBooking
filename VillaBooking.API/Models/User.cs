using System.ComponentModel.DataAnnotations;

namespace VillaBooking.API.Models
{
    public class User : BaseEntity
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        public string NormalizedEmail { get; set; } = null!;
        public required string Name { get; set; }

        public string PasswordHash { get; set; } = null!;

        public required string Role { get; set; } = "Customer";

        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }
    }
}
