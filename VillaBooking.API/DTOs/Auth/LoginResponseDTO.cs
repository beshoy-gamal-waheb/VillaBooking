namespace VillaBooking.API.DTOs.Auth
{
    public class LoginResponseDTO
    {
        public string? Token { get; set; }
        public UserDTO? UserDTO { get; set; }
    }
}
