using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsAggregation.Server.Models.Entities
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Column(TypeName = "text")]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string NotificationType { get; set; } = "General"; 

        public int? RelatedArticleId { get; set; }

        public virtual User User { get; set; } = null!;

        [ForeignKey("RelatedArticleId")]
        public virtual NewsArticle? RelatedArticle { get; set; }
    }
}