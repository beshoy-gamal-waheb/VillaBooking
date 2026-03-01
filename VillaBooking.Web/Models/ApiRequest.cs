using static VillaBooking.Web.SD;

namespace VillaBooking.Web.Models
{
    public class ApiRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string? URL { get; set; }
        public object? Data { get; set; }
        public string? Token { get; set; }
    }
}
