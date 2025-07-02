using System.ComponentModel.DataAnnotations;

namespace NewsAggregation.Server.Models.Dtos.News
{
    public class ReportArticleDto
    {
        [Required]
        public int ArticleId { get; set; }

        [StringLength(255)]
        public string? Reason { get; set; }
    }
} 