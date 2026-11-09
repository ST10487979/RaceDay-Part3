using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class RegisterViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Participant";

        // Ensure these are here and spelled exactly like this:
        public DateTime? DateOfBirth { get; set; }
        public string? Province { get; set; }
        public string? EmergencyContact { get; set; }
    }
}