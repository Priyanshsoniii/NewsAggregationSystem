using System.ComponentModel.DataAnnotations;

namespace NewsAggregation.Server.Models.Dtos.User
{
    public class UpdateUserProfileDto
    {
        [StringLength(100)]
        public string? Username { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        [MinLength(6)]
        public string? CurrentPassword { get; set; }

        [MinLength(6)]
        public string? NewPassword { get; set; }
    }
} 