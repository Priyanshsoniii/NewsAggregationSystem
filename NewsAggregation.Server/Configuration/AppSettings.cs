namespace NewsAggregation.Server.Configuration
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = new();
        public JwtSettings JwtSettings { get; set; } = new();
        public EmailSettings EmailSettings { get; set; } = new();
        public NewsApiSettings NewsApiSettings { get; set; } = new();
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; } = "Server=ITT-PRIYANSH-S;Initial Catalog=NewsAggregationDB2;Trusted_Connection=True;Integrated Security=True;TrustServerCertificate=True";
    }

    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationHours { get; set; } = 24;
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
    }

    public class NewsApiSettings
    {
        public string NewsApiKey { get; set; } = string.Empty;
        public string TheNewsApiKey { get; set; } = string.Empty;
        public string FirebaseApiKey { get; set; } = string.Empty;
        public int FetchIntervalHours { get; set; } = 3;
    }
}