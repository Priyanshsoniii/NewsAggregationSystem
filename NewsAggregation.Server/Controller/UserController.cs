using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Server.Models.Dtos.User;
using NewsAggregation.Server.Services.Interfaces;
using System.Security.Claims;

namespace NewsAggregation.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: api/User/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                    return NotFound(new { Message = "User not found" });

                return Ok(new
                {
                    Success = true,
                    User = new
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                        user.Role,
                        user.CreatedAt,
                        user.IsActive
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile");
                return StatusCode(500, new { Message = "An error occurred while retrieving user profile" });
            }
        }

        // PUT: api/User/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                    return NotFound(new { Message = "User not found" });

                if (!string.IsNullOrEmpty(updateDto.Username))
                {
                    var existingUser = await _userService.GetUserByUsernameAsync(updateDto.Username);
                    if (existingUser != null && existingUser.Id != userId)
                        return Conflict(new { Message = "Username already taken" });
                    
                    user.Username = updateDto.Username;
                }

                if (!string.IsNullOrEmpty(updateDto.Email))
                {
                    var existingUser = await _userService.GetUserByEmailAsync(updateDto.Email);
                    if (existingUser != null && existingUser.Id != userId)
                        return Conflict(new { Message = "Email already taken" });
                    
                    user.Email = updateDto.Email;
                }

                if (!string.IsNullOrEmpty(updateDto.CurrentPassword) && !string.IsNullOrEmpty(updateDto.NewPassword))
                {
                    if (!BCrypt.Net.BCrypt.Verify(updateDto.CurrentPassword, user.PasswordHash))
                        return BadRequest(new { Message = "Current password is incorrect" });
                    
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateDto.NewPassword);
                }

                var updatedUser = await _userService.UpdateUserAsync(user);

                return Ok(new
                {
                    Success = true,
                    Message = "Profile updated successfully",
                    User = new
                    {
                        updatedUser.Id,
                        updatedUser.Username,
                        updatedUser.Email,
                        updatedUser.Role,
                        updatedUser.CreatedAt,
                        updatedUser.IsActive
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return StatusCode(500, new { Message = "An error occurred while updating user profile" });
            }
        }

        // DELETE: api/User/profile
        [HttpDelete("profile")]
        public async Task<IActionResult> DeleteUserProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var result = await _userService.DeleteUserAsync(userId);
                if (!result)
                    return NotFound(new { Message = "User not found" });

                return Ok(new
                {
                    Success = true,
                    Message = "User profile deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user profile");
                return StatusCode(500, new { Message = "An error occurred while deleting user profile" });
            }
        }

        // GET: api/User/check-email
        [HttpGet("check-email")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEmailAvailability([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return BadRequest(new { Message = "Email is required" });

                var existingUser = await _userService.GetUserByEmailAsync(email);
                var isAvailable = existingUser == null;

                return Ok(new
                {
                    Success = true,
                    Email = email,
                    IsAvailable = isAvailable,
                    Message = isAvailable ? "Email is available" : "Email is already taken"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email availability");
                return StatusCode(500, new { Message = "An error occurred while checking email availability" });
            }
        }

        // GET: api/User/check-username
        [HttpGet("check-username")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckUsernameAvailability([FromQuery] string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                    return BadRequest(new { Message = "Username is required" });

                var existingUser = await _userService.GetUserByUsernameAsync(username);
                var isAvailable = existingUser == null;

                return Ok(new
                {
                    Success = true,
                    Username = username,
                    IsAvailable = isAvailable,
                    Message = isAvailable ? "Username is available" : "Username is already taken"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking username availability");
                return StatusCode(500, new { Message = "An error occurred while checking username availability" });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }
    }
}
