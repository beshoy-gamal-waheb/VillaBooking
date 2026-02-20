using System.ComponentModel.DataAnnotations;

namespace VillaBooking.API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public required string Role { get; set; } = "Customer";

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }
    }
}
