using System.ComponentModel.DataAnnotations;

namespace VillaBooking.DTO.Auth
{
    public class UserDTO
    {
        public string Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
