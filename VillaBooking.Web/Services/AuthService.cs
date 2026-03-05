using VillaBooking.DTO.Auth;
using VillaBooking.Web.Models;
using VillaBooking.Web.Services.IServices;

namespace VillaBooking.Web.Services
{
    public class AuthService(IHttpClientFactory _httpClient, IHttpContextAccessor _httpContext)
        : BaseService(_httpClient, _httpContext), IAuthService
    {
        private const string APIEndpoint = "/api/auth";

        public Task<T?> LoginAsync<T>(LoginRequestDTO loginRequestDTO)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDTO,
                URL = $"{APIEndpoint}/login"
            });
        }

        public Task<T?> RegisterAsync<T>(RegisterationRequestDTO registerationRequestDTO)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = registerationRequestDTO,
                URL = $"{APIEndpoint}/register"
            });
        }
    }
}
