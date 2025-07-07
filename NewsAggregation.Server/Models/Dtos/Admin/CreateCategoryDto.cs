using System.ComponentModel.DataAnnotations;

namespace NewsAggregation.Server.Models.Dtos.Admin
{
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }
    }
} 