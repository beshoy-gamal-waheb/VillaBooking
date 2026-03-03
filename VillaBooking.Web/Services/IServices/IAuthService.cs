using VillaBooking.DTO.Auth;

namespace VillaBooking.Web.Services.IServices
{
    public interface IAuthService
    {
        Task<T?> LoginAsync<T>(LoginRequestDTO loginRequestDTO);
        Task<T?> RegisterAsync<T>(RegisterationRequestDTO registerationRequestDTO);
    }
}
