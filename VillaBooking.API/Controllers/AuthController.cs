using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using VillaBooking.API.DTOs.Auth;
using VillaBooking.API.Exceptions;
using VillaBooking.API.Models.Responses;
using VillaBooking.API.Services.Auth;

namespace VillaBooking.API.Controllers
{
    [AllowAnonymous]
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService _authService) : ControllerBase
    {
        [HttpPost("register")]
        [ProducesResponseType(typeof(APIResponse<UserDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<UserDTO>>> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            try
            {
                if (registerationRequestDTO is null)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Registeration data is required"));
                }

                if (await _authService.IsEmailExistsAsync(registerationRequestDTO.Email))
                {
                    return Conflict(APIResponse<object>.Conflict($"User with email {registerationRequestDTO.Email} already exists"));
                }

                var user = await _authService.RegisterAsync(registerationRequestDTO);
                if (user is null)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Registeration failed"));
                }

                var response = APIResponse<UserDTO>.CreatedAt(user, "User registered successfully");
                return CreatedAtAction(nameof(Register), response);

            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(StatusCodes.Status500InternalServerError,
                                                              "An error occurred during registration",
                                                              ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }


        [HttpPost("login")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<LoginResponseDTO>>> Login(LoginRequestDTO loginRequestDTO)
        {
            try
            {
                if (loginRequestDTO is null)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Login data is required"));
                }

                var loginResponseDTO = await _authService.LoginAsync(loginRequestDTO);
                if (loginResponseDTO is null)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Invalid email or password"));
                }

                var response = APIResponse<LoginResponseDTO>.Ok(loginResponseDTO, "login successfully");
                return Ok(response);
            }
            catch (AccountLockedException ex)
            {
                var errorResponse = APIResponse<object>.Locked(ex.Message);
                return StatusCode(StatusCodes.Status423Locked, errorResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(StatusCodes.Status500InternalServerError,
                                                              "An error occurred during Login",
                                                               ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
    }
}
