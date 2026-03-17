using VillaBooking.DTO.Villa;
using VillaBooking.Web.Extensions;
using VillaBooking.Web.Models;
using VillaBooking.Web.Services.IServices;

namespace VillaBooking.Web.Services
{
    public class VillaService(IHttpClientFactory _httpClient, IHttpContextAccessor _httpContext)
        : BaseService(_httpClient, _httpContext), IVillaService
    {
        private const string ApiEndpoint = $"/api/{SD.CurrentApiVersion}/villa";

        public Task<T?> GetAllAsync<T>()
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                URL = ApiEndpoint
            });
        }

        public Task<T?> GetAllAsync<T>(VillaQueryParameters parameters)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                URL = $"{ApiEndpoint}{parameters.ToQueryString()}"
            });
        }

        public Task<T?> GetAsync<T>(int id)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                URL = $"{ApiEndpoint}/{id}"
            });
        }

        public Task<T?> CreateAsync<T>(VillaUpsertDTO dto)
        {
            var formData = dto.ToMultipartFormData();
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                URL = ApiEndpoint,
                Data = formData
            });
        }

        public Task<T?> UpdateAsync<T>(int id, VillaUpsertDTO dto)
        {
            var formData = dto.ToMultipartFormData();
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.PUT,
                URL = $"{ApiEndpoint}/{id}",
                Data = formData
            });
        }

        public Task<T?> DeleteAsync<T>(int id)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.DELETE,
                URL = $"{ApiEndpoint}/{id}"
            });
        }

    }
}
