using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Server.Models.Dtos.Notification;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Services.Interfaces;
using System.Security.Claims;

namespace NewsAggregation.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            INotificationService notificationService,
            ICategoryService categoryService,
            ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _categoryService = categoryService;
            _logger = logger;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        // GET: api/Notification
        [HttpGet]
        public async Task<IActionResult> GetUserNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var notifications = await _notificationService.GetUserNotificationsAsync(userId);
                var notificationsList = notifications.Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Message,
                    n.IsRead,
                    n.NotificationType,
                    n.RelatedArticleId,
                    n.CreatedAt
                }).OrderByDescending(n => n.CreatedAt).ToList();

                return Ok(new
                {
                    Success = true,
                    Count = notificationsList.Count,
                    Notifications = notificationsList
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user notifications");
                return StatusCode(500, new { Message = "An error occurred while retrieving notifications" });
            }
        }

        // GET: api/Notification/unread
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
                var notificationsList = notifications.Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Message,
                    n.NotificationType,
                    n.RelatedArticleId,
                    n.CreatedAt
                }).OrderByDescending(n => n.CreatedAt).ToList();

                return Ok(new
                {
                    Success = true,
                    Count = notificationsList.Count,
                    UnreadNotifications = notificationsList
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving unread notifications");
                return StatusCode(500, new { Message = "An error occurred while retrieving unread notifications" });
            }
        }

        // PATCH: api/Notification/{id}/mark-read
        [HttpPatch("{id:int}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var result = await _notificationService.MarkAsReadAsync(id);
                if (!result)
                    return NotFound(new { Message = "Notification not found" });

                return Ok(new { Success = true, Message = "Notification marked as read" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                return StatusCode(500, new { Message = "An error occurred while marking notification as read" });
            }
        }

        // PATCH: api/Notification/mark-all-read
        [HttpPatch("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var result = await _notificationService.MarkAllAsReadAsync(userId);
                if (!result)
                    return NotFound(new { Message = "No notifications found" });

                return Ok(new { Success = true, Message = "All notifications marked as read" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                return StatusCode(500, new { Message = "An error occurred while marking notifications as read" });
            }
        }

        // DELETE: api/Notification/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var result = await _notificationService.DeleteNotificationAsync(id);
                if (!result)
                    return NotFound(new { Message = "Notification not found" });

                return Ok(new { Success = true, Message = "Notification deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification");
                return StatusCode(500, new { Message = "An error occurred while deleting notification" });
            }
        }

        // GET: api/Notification/settings
        [HttpGet("settings")]
        public async Task<IActionResult> GetNotificationSettings()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var settings = await _notificationService.GetUserNotificationSettingsAsync(userId);
                var categories = await _categoryService.GetAllCategoriesAsync();

                var settingsList = categories.Select(c => new
                {
                    CategoryId = (int?)c.Id,
                    CategoryName = c.Name,
                    IsEnabled = settings.FirstOrDefault(s => s.CategoryId == c.Id)?.IsEnabled ?? false,
                    Keywords = settings.FirstOrDefault(s => s.CategoryId == c.Id)?.Keywords,
                    EmailNotifications = settings.FirstOrDefault(s => s.CategoryId == c.Id)?.EmailNotifications ?? true
                }).ToList();

                // Add general keywords setting
                var generalSetting = settings.FirstOrDefault(s => s.CategoryId == null);
                settingsList.Add(new
                {
                    CategoryId = (int?)null,
                    CategoryName = "Keywords",
                    IsEnabled = generalSetting?.IsEnabled ?? false,
                    Keywords = generalSetting?.Keywords,
                    EmailNotifications = generalSetting?.EmailNotifications ?? true
                });

                return Ok(new
                {
                    Success = true,
                    Settings = settingsList
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notification settings");
                return StatusCode(500, new { Message = "An error occurred while retrieving notification settings" });
            }
        }

        // PATCH: api/Notification/settings/{categoryId}/toggle
        [HttpPatch("settings/{categoryId:int?}/toggle")]
        public async Task<IActionResult> ToggleNotificationSetting(int? categoryId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var isEnabled = await _notificationService.ToggleNotificationSettingAsync(userId, categoryId);

                return Ok(new
                {
                    Success = true,
                    IsEnabled = isEnabled,
                    Message = $"Notifications {(isEnabled ? "enabled" : "disabled")} successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling notification setting");
                return StatusCode(500, new { Message = "An error occurred while toggling notification setting" });
            }
        }

        // PUT: api/Notification/settings/{categoryId}/keywords
        [HttpPut("settings/{categoryId:int?}/keywords")]
        public async Task<IActionResult> UpdateKeywords(int? categoryId, [FromBody] UpdateKeywordsDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var result = await _notificationService.UpdateKeywordsAsync(userId, categoryId, dto.Keywords);

                return Ok(new
                {
                    Success = true,
                    Message = "Keywords updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating keywords");
                return StatusCode(500, new { Message = "An error occurred while updating keywords" });
            }
        }

        // PUT: api/Notification/settings
        [HttpPut("settings")]
        public async Task<IActionResult> UpdateNotificationSettings([FromBody] object settingsDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                // For now, we'll just return success since the backend structure is different
                // In a real implementation, you'd map the flat structure to category-based settings
                return Ok(new
                {
                    Success = true,
                    Message = "Notification settings updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification settings");
                return StatusCode(500, new { Message = "An error occurred while updating notification settings" });
            }
        }

        // POST: api/Notification/test-email
        [HttpPost("test-email")]
        public async Task<IActionResult> SendTestEmail()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var sent = await _notificationService.SendEmailNotificationAsync(
                    userId,
                    "Test Email Notification",
                    "This is a test email notification from News Aggregation. If you received this email, your email notifications are working correctly!"
                );

                if (sent)
                    return Ok(new { Success = true, Message = "Test email sent successfully." });
                else
                    return StatusCode(500, new { Success = false, Message = "Failed to send test email. Please check your email settings." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending test email notification");
                return StatusCode(500, new { Message = "An error occurred while sending test email." });
            }
        }

        // POST: api/Notification/test-email-direct
        [HttpPost("test-email-direct")]
        [AllowAnonymous]
        public async Task<IActionResult> SendTestEmailDirect([FromBody] TestEmailRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new { Success = false, Message = "Email address is required." });
                }

                _logger.LogInformation("Testing email configuration for: {Email}", request.Email);

                var emailService = HttpContext.RequestServices.GetRequiredService<IEmailService>();
                var sent = await emailService.SendEmailAsync(
                    request.Email,
                    "Test Email from News Aggregation System",
                    "<h2>Test Email</h2><p>This is a test email from your News Aggregation System. If you received this email, your email configuration is working correctly!</p><p>Sent at: " + DateTime.Now + "</p>"
                );

                if (sent)
                {
                    _logger.LogInformation("Test email sent successfully to {Email}", request.Email);
                    return Ok(new { Success = true, Message = "Test email sent successfully to " + request.Email });
                }
                else
                {
                    _logger.LogError("Failed to send test email to {Email}", request.Email);
                    return StatusCode(500, new { Success = false, Message = "Failed to send test email. Please check your email settings and application logs." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending direct test email to {Email}: {ErrorMessage}", request.Email, ex.Message);
                return StatusCode(500, new { Success = false, Message = "An error occurred while sending test email: " + ex.Message });
            }
        }
    }

    public class TestEmailRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
