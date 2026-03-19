using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VillaBooking.Web.Services.IServices;

namespace VillaBooking.Web.Services
{
    public class TokenProvider(IHttpContextAccessor _httpContextAccessor) : ITokenProvider
    {
        public void SetToken(string token)
        {
            _httpContextAccessor.HttpContext?.Session.SetString(SD.SessionToken, token);
        }
        public string? GetToken()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString(SD.SessionToken);
        }

        public void ClearToken()
        {
            _httpContextAccessor.HttpContext?.Session.Remove(SD.SessionToken);
        }

        public ClaimsPrincipal? CreatePrincipalFromJwtToken(string token)
        {
            if(token is null)
            {
                return null;
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

                var emailClaim = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                if (emailClaim is not null)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Name, emailClaim));
                }

                var roleClaim = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
                if (roleClaim is not null)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim));
                }

                var nameClaim = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                if (roleClaim is not null)
                {
                    identity.AddClaim(new Claim("FullName", roleClaim));
                }

                return new ClaimsPrincipal(identity);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
