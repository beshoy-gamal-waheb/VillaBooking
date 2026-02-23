namespace VillaBooking.DTO.Auth
{
    public class LoginResponseDTO
    {
        public string? Token { get; set; }
        public UserDTO? UserDTO { get; set; }
    }
}
