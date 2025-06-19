using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Server.Dtos;
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
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.LoginAsync(loginDto.Username, loginDto.Password);

                if (!string.IsNullOrEmpty(response.Token))
                {
                    var user = await _userService.GetUserByEmailAsync(loginDto.Username);

                    return Ok(new
                    {
                        Token = response,
                        User = user,
                        Message = "Login successful"
                    });
                }

                return Unauthorized(new { Message = "Invalid email or password" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during login", Error = ex.Message });
            }
        }

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
                        Message = "User registered successfully",
                        Email = registerDto.Email
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
