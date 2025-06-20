//using Microsoft.AspNetCore.Mvc;
//using NewsAggregation.Server.Services.Interfaces;
//using System.ComponentModel.DataAnnotations;

//namespace NewsAggregation.Server.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class EmailController : ControllerBase
//    {
//        private readonly IEmailService _emailService;

//        public EmailController(IEmailService emailService)
//        {
//            _emailService = emailService;
//        }

//        // POST: api/Email/send
//        [HttpPost("send")]
//        public async Task<IActionResult> SendEmail([FromBody] SendEmailDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _emailService.SendEmailAsync(dto.ToEmail, dto.Subject, dto.Body);

//            if (result)
//                return Ok(new { Message = "Email sent successfully" });

//            return StatusCode(500, new { Message = "Failed to send email" });
//        }

//        // POST: api/Email/send-news-notification
//        [HttpPost("send-news-notification")]
//        public async Task<IActionResult> SendNewsNotification([FromBody] SendNewsNotificationDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _emailService.SendNewsNotificationAsync(dto.ToEmail, dto.UserName, dto.NewsTitle, dto.NewsUrl);

//            if (result)
//                return Ok(new { Message = "News notification sent successfully" });

//            return StatusCode(500, new { Message = "Failed to send news notification" });
//        }

//        // POST: api/Email/send-welcome
//        [HttpPost("send-welcome")]
//        public async Task<IActionResult> SendWelcomeEmail([FromBody] SendWelcomeEmailDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _emailService.SendWelcomeEmailAsync(dto.ToEmail, dto.UserName);

//            if (result)
//                return Ok(new { Message = "Welcome email sent successfully" });

//            return StatusCode(500, new { Message = "Failed to send welcome email" });
//        }
//    }

//    public class SendEmailDto
//    {
//        [Required, EmailAddress]
//        public string ToEmail { get; set; } = string.Empty;

//        [Required]
//        public string Subject { get; set; } = string.Empty;

//        [Required]
//        public string Body { get; set; } = string.Empty;
//    }

//    public class SendNewsNotificationDto
//    {
//        [Required, EmailAddress]
//        public string ToEmail { get; set; } = string.Empty;

//        [Required]
//        public string UserName { get; set; } = string.Empty;

//        [Required]
//        public string NewsTitle { get; set; } = string.Empty;

//        [Required, Url]
//        public string NewsUrl { get; set; } = string.Empty;
//    }

//    public class SendWelcomeEmailDto
//    {
//        [Required, EmailAddress]
//        public string ToEmail { get; set; } = string.Empty;

//        [Required]
//        public string UserName { get; set; } = string.Empty;
//    }
//}
