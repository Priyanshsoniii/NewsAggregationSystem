using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using NewsAggregation.Server.Configuration;
using NewsAggregation.Server.Services.Interfaces;

namespace NewsAggregation.Server.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<AppSettings> appSettings, ILogger<EmailService> logger)
        {
            _emailSettings = appSettings.Value.EmailSettings;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendNewsNotificationAsync(string toEmail, string userName, string newsTitle, string newsUrl)
        {
            var subject = "News Alert - " + newsTitle;
            var body = $@"
                <html>
                <body>
                    <h2>Hello {userName},</h2>
                    <p>We found a news article that matches your interests:</p>
                    <h3>{newsTitle}</h3>
                    <p><a href='{newsUrl}' target='_blank'>Read Full Article</a></p>
                    <p>Best regards,<br>News Aggregation Team</p>
                </body>
                </html>";

            return await SendEmailAsync(toEmail, subject, body);
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var subject = "Welcome to News Aggregation System";
            var body = $@"
                <html>
                <body>
                    <h2>Welcome {userName}!</h2>
                    <p>Thank you for joining our News Aggregation System.</p>
                    <p>You can now:</p>
                    <ul>
                        <li>Browse latest news headlines</li>
                        <li>Save articles for later reading</li>
                        <li>Configure notifications for topics of interest</li>
                        <li>Search through our news database</li>
                    </ul>
                    <p>Best regards,<br>News Aggregation Team</p>
                </body>
                </html>";

            return await SendEmailAsync(toEmail, subject, body);
        }
    }
}