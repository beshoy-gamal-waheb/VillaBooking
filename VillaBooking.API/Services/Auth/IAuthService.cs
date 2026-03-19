using VillaBooking.DTO.Auth;

namespace VillaBooking.API.Services.Auth
{
    public interface IAuthService
    {
        Task<UserDTO?> RegisterAsync(RegisterationRequestDTO registerationRequestDTO);
        Task<TokenDTO?> LoginAsync(LoginRequestDTO loginRequestDTO);
        Task<bool> IsEmailExistsAsync(string email);
    }
}
