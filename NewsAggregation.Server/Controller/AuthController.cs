using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Server.Models.Dtos;
using NewsAggregation.Server.Services.Interfaces;

namespace NewsAggregation.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthManagementController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthManagementController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, token, user) = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

            if (!success)
                return Unauthorized(new { Message = "Invalid email/username or password" });

            return Ok(new
            {
                Success = true,
                Token = token,
                User = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.Role,
                    user.CreatedAt,
                    user.IsActive
                },
                Message = "Login successful"
            });
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUser = await _userService.GetUserByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    return Conflict(new { Message = "User with this email already exists" });
                }

                var allUsers = await _userService.GetAllUsersAsync();
                if (allUsers.Any(u => u.Username == registerDto.Username))
                {
                    return Conflict(new { Message = "Username already taken" });
                }

                var registrationResult = await _authService.RegisterAsync(
                    registerDto.Username,
                    registerDto.Email,
                    registerDto.Password
                );

                if (registrationResult.Success)
                {
                    return CreatedAtAction(nameof(Login), new
                    {
                        Success = true,
                        Message = "User registered successfully",
                        User = new
                        {
                            registrationResult.User!.Id,
                            registrationResult.User.Username,
                            registrationResult.User.Email,
                            registrationResult.User.Role,
                            registrationResult.User.CreatedAt,
                            registrationResult.User.IsActive
                        }
                    });
                }

                return BadRequest(new { Message = registrationResult.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during registration", Error = ex.Message });
            }
        }
    }
}
