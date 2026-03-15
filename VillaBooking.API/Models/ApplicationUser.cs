using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace VillaBooking.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public required string FullName { get; set; }
    }
}
