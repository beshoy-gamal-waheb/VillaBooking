using System.Net.Http.Headers;
using System.Text.Json;
using VillaBooking.DTO.Responses;
using VillaBooking.Web.Models;
using VillaBooking.Web.Services.IServices;

namespace VillaBooking.Web.Services
{
    public class BaseService : IBaseService
    {
        public IHttpClientFactory HttpClient { get; set; }
        public APIResponse<object> ResponseModel { get; set; }

        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClient, ITokenProvider tokenProvider)
        {
            this.ResponseModel = new();
            this.HttpClient = httpClient;
            _tokenProvider = tokenProvider;
        }

        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
 

        public async Task<T?> SendAsync<T>(ApiRequest apiRequest, bool withBearer = true)
        {
            try
            {
                var client = HttpClient.CreateClient("VillaBookingAPI");
                var message = new HttpRequestMessage()
                {
                    RequestUri = new Uri(apiRequest.URL, uriKind: UriKind.Relative),
                    Method = GetHttpMethod(apiRequest.ApiType)
                };

                var token = _tokenProvider.GetToken();
                if (withBearer && !string.IsNullOrEmpty(token))
                {
                    message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                if (apiRequest.Data is not null)
                {
                    if (apiRequest.Data is MultipartFormDataContent multipartForm)
                    {
                        message.Content = multipartForm;
                    }
                    else
                    {
                        message.Content = JsonContent.Create(apiRequest.Data, options: jsonOptions);
                    }
                }
                    
                var apiResponse = await client.SendAsync(message);

                return await apiResponse.Content.ReadFromJsonAsync<T>(jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return default;
            }
        }
        private static HttpMethod GetHttpMethod(SD.ApiType apiType)
        {
            return apiType switch
            {
                SD.ApiType.POST => HttpMethod.Post,
                SD.ApiType.PUT => HttpMethod.Put,
                SD.ApiType.DELETE => HttpMethod.Delete,
                _ => HttpMethod.Get,
            };
        }
    }
}
