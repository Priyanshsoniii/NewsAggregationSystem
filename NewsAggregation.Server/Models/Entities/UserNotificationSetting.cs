using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsAggregation.Server.Models.Entities
{
    public class UserNotificationSetting
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Category")]
        public int? CategoryId { get; set; }

        public bool IsEnabled { get; set; } = true;

        [Column(TypeName = "text")]
        public string? Keywords { get; set; } // JSON array of keywords

        public bool EmailNotifications { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Category? Category { get; set; }
    }
}