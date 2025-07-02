using System.ComponentModel.DataAnnotations;

namespace NewsAggregation.Server.Models.Entities
{
    public class FilteredKeyword
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Keyword { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
} 