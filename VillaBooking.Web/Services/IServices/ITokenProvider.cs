using System.Security.Claims;

namespace VillaBooking.Web.Services.IServices
{
    public interface ITokenProvider
    {
        void SetToken(string token);
        string? GetToken();
        void ClearToken();
        ClaimsPrincipal? CreatePrincipalFromJwtToken(string token);
    }
}
