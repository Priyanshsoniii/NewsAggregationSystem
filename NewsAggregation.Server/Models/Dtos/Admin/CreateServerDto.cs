using System.ComponentModel.DataAnnotations;

namespace NewsAggregation.Server.Models.Dtos.Admin
{
    public class CreateServerDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Url]
        public string ApiUrl { get; set; } = string.Empty;

        [Required]
        public string ApiKey { get; set; } = string.Empty;

        [Required]
        public string ServerType { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public int RequestsPerHour { get; set; } = 1000;
    }
} 