using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using VillaBooking.DTO.Auth;
using VillaBooking.DTO.Responses;
using VillaBooking.Web.Services.IServices;

namespace VillaBooking.Web.Controllers
{
    public class AuthController(IAuthService _authService, ITokenProvider _tokenProvider) : Controller
    {
        #region Login
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
                
            try
            {
                var response = await _authService.LoginAsync<APIResponse<TokenDTO>>(model);

                if (response == null || !response.Success || response.Data == null)
                {
                    ModelState.AddModelError(string.Empty, response?.Message ?? "Login failed. Please try again.");
                    return View(model);
                }

                var token = response.Data.AccessToken;

                if (string.IsNullOrWhiteSpace(token))
                {
                    ModelState.AddModelError(string.Empty, "Invalid token received.");
                    return View(model);
                }

                
                var principal = _tokenProvider.CreatePrincipalFromJwtToken(token);
                if (principal is not null)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    _tokenProvider.SetToken(token);
                }

                //HttpContext.Session.SetString(SD.SessionToken, token);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
            }
            return View(model);
        }

        #endregion

        #region Register

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new RegisterationRequestDTO
            {
                Email = string.Empty,
                FullName = string.Empty,
                Password = string.Empty,
                Role = "Customer"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(registerationRequestDTO);
            }

            try
            {
                var response = await _authService.RegisterAsync<APIResponse<UserDTO>>(registerationRequestDTO);
                if (response != null && response.Success && response.Data != null)
                {
                    TempData["success"] = "Registration successful! Please log in.";
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response?.Message ?? "Registration failed. Please try again.");
                    return View(registerationRequestDTO);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred during registration: {ex.Message}";
            }
            return View(registerationRequestDTO);
        }
        #endregion

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _tokenProvider.ClearToken();
            return RedirectToAction(nameof(Login));
        }
    }
}
