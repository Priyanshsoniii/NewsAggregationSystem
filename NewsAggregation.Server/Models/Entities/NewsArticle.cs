using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsAggregation.Server.Models.Entities
{
    public class NewsArticle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Title { get; set; } = string.Empty;

        [Column(TypeName = "text")]
        public string? Description { get; set; }

        [Required]
        [Url]
        public string Url { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Source { get; set; } = string.Empty;

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public DateTime PublishedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        [StringLength(100)]
        public string? Author { get; set; }

        public int Likes { get; set; } = 0;

        public int Dislikes { get; set; } = 0;

        // Navigation Properties
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<SavedArticle> SavedByUsers { get; set; } = new List<SavedArticle>();
    }
}