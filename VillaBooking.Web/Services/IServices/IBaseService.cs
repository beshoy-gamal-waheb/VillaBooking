using VillaBooking.DTO.Responses;
using VillaBooking.Web.Models;

namespace VillaBooking.Web.Services.IServices
{
    public interface IBaseService
    {
        APIResponse<object> ResponseModel { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
