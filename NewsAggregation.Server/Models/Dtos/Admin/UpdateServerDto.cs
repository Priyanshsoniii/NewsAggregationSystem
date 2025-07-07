namespace NewsAggregation.Server.Models.Dtos.Admin
{
    public class UpdateServerDto
    {
        public string? Name { get; set; }
        public string? ApiUrl { get; set; }
        public string? ApiKey { get; set; }
        public string? ServerType { get; set; }
        public bool? IsActive { get; set; }
        public int? RequestsPerHour { get; set; }
    }
} 