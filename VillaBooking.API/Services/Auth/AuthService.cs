using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VillaBooking.API.Data.Contexts;
using VillaBooking.API.DTOs.Auth;
using VillaBooking.API.Exceptions;
using VillaBooking.API.Models;
using VillaBooking.API.Models.Responses;

namespace VillaBooking.API.Services.Auth
{
    public class AuthService(ApplicationDbContext _dbContext, IMapper _mapper,
                             IPasswordHasher<User> _passwordHasher, IConfiguration _configuration) : IAuthService
    {
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            var normalizedEmail = email.ToUpper();
            return await _dbContext.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail);
        }

        public async Task<UserDTO?> RegisterAsync(RegisterationRequestDTO registerationRequestDTO)
        {
            try
            {
                if (await IsEmailExistsAsync(registerationRequestDTO.Email))
                {
                    throw new InvalidOperationException($"User with email {registerationRequestDTO.Email} already exists");
                }

                User user = new()
                {
                    Email = registerationRequestDTO.Email,
                    NormalizedEmail = registerationRequestDTO.Email.ToUpper(),
                    Name = registerationRequestDTO.Name,
                    Role = string.IsNullOrWhiteSpace(registerationRequestDTO.Role) ? "Customer" : registerationRequestDTO.Role,
                    CreatedDate = DateTime.Now,
                };

                user.PasswordHash = _passwordHasher.HashPassword(user, registerationRequestDTO.Password);

                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();

                return _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An unexpected error occurred during user registeration", ex);
            }
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            var normalizedEmail = loginRequestDTO.Email.ToUpper();
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
            if (user is null)
            {
                return null;
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                var localTime = user.LockoutEnd.Value.ToLocalTime();
                throw new AccountLockedException($"Account is locked untill {localTime}"); 
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequestDTO.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                    user.FailedLoginAttempts = 0;
                }

                await _dbContext.SaveChangesAsync();
                return null;
            }

            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            await _dbContext.SaveChangesAsync();

            //Generate Token
            var token = GenerateToken(user);

            return new LoginResponseDTO()
                {
                    Token = token,
                    UserDTO = _mapper.Map<UserDTO>(user)
                };
        }

        private string GenerateToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtSettings")["Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]{
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
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
