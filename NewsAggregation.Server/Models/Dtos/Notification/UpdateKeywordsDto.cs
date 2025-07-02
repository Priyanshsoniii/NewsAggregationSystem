using System.ComponentModel.DataAnnotations;

namespace NewsAggregation.Server.Models.Dtos.Notification
{
    public class UpdateKeywordsDto
    {
        [Required]
        public List<string> Keywords { get; set; } = new List<string>();
    }
} 