using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VillaBooking.API.Data.Contexts;
using VillaBooking.API.Exceptions;
using VillaBooking.API.Models;
using VillaBooking.DTO.Auth;
using VillaBooking.DTO.Responses;

namespace VillaBooking.API.Services.Auth
{
    public class AuthService(ApplicationDbContext _dbContext, IMapper _mapper, IConfiguration _configuration,
        UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager,
        SignInManager<ApplicationUser> _signInManager) : IAuthService
    {
        private const string DefaultRole = "Customer";
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }

        public async Task<UserDTO?> RegisterAsync(RegisterationRequestDTO registerationRequestDTO)
        {
            if (await IsEmailExistsAsync(registerationRequestDTO.Email))
            {
                throw new InvalidOperationException($"User with email '{registerationRequestDTO.Email}' already exists");
            }

            ApplicationUser user = new()
            {
                Email = registerationRequestDTO.Email,
                UserName = registerationRequestDTO.Email,
                FullName = registerationRequestDTO.FullName,
                EmailConfirmed = true,
            };

            //user.PasswordHash = _passwordHasher.HashPassword(user, registerationRequestDTO.Password);
            var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create user: {errors}");
            }

            var role = string.IsNullOrWhiteSpace(registerationRequestDTO.Role) ? DefaultRole : registerationRequestDTO.Role;
            if (!await _roleManager.RoleExistsAsync(role))
            {
                throw new InvalidOperationException($"Role '{role}' does not exist.");
            }

            await _userManager.AddToRoleAsync(user, role);

            var userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Role = role;

            return userDTO;
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDTO.Email);
            if (user is null)
            {
                return null;
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                var localTime = user.LockoutEnd.Value.ToLocalTime();
                throw new AccountLockedException($"Account is locked until {localTime.ToString("f", CultureInfo.GetCultureInfo("en-US"))}"); 
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginRequestDTO.Password, true);
            if (!signInResult.Succeeded)
            {
                return null;
            }

            //Generate Token
            var token = await GenerateToken(user);

            return new LoginResponseDTO()
                {
                    Token = token,
                    UserDTO = _mapper.Map<UserDTO>(user)
                };
        }

        private async Task<string> GenerateToken(ApplicationUser user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtSettings")["Secret"]);
            var userRoles = await _userManager.GetRolesAsync(user);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]{
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim(ClaimTypes.Role, userRoles.FirstOrDefault() ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
