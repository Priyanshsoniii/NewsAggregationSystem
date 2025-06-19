namespace NewsAggregation.Server.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body);
        Task<bool> SendNewsNotificationAsync(string toEmail, string userName, string newsTitle, string newsUrl);
        Task<bool> SendWelcomeEmailAsync(string toEmail, string userName);
    }
}