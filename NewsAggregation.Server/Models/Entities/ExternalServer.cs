using System.ComponentModel.DataAnnotations;

namespace NewsAggregation.Server.Models.Entities
{
    public class ExternalServer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Url]
        public string ApiUrl { get; set; } = string.Empty;

        [Required]
        public string ApiKey { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime? LastAccessed { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string ServerType { get; set; } = string.Empty;

        public int RequestsPerHour { get; set; } = 1000;

        public int CurrentHourRequests { get; set; } = 0;

        public DateTime LastHourReset { get; set; } = DateTime.UtcNow;
    }
}