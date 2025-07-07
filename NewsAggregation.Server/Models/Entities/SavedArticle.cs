using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsAggregation.Server.Models.Entities
{
    public class SavedArticle
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("NewsArticle")]
        public int NewsArticleId { get; set; }

        public DateTime SavedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;
        public virtual NewsArticle NewsArticle { get; set; } = null!;
    }
}