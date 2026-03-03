using VillaBooking.DTO.Villa;
using VillaBooking.Web.Models;
using VillaBooking.Web.Services.IServices;

namespace VillaBooking.Web.Services
{
    public class VillaService(IHttpClientFactory _httpClient) : BaseService(_httpClient), IVillaService
    {
        private const string ApiEndpoint = "/api/villa";

        public Task<T?> GetAllAsync<T>(string token)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                URL = ApiEndpoint,
                Token = token
            });
        }

        public Task<T?> GetAsync<T>(int id, string token)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                URL = $"{ApiEndpoint}/{id}",
                Token = token
            });
        }

        public Task<T?> CreateAsync<T>(VillaUpsertDTO dto, string token)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                URL = ApiEndpoint,
                Data = dto,
                Token = token
            });
        }

        public Task<T?> UpdateAsync<T>(int id, VillaUpsertDTO dto, string token)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.PUT,
                URL = $"{ApiEndpoint}/{id}",
                Data = dto,
                Token = token
            });
        }

        public Task<T?> DeleteAsync<T>(int id, string token)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.DELETE,
                URL = $"{ApiEndpoint}/{id}",
                Token = token
            });
        }

    }
}
