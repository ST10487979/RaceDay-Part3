using System.ComponentModel.DataAnnotations;

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

    // ADD THESE THREE LINES:
    public DateTime? DateOfBirth { get; set; } = new DateTime(1990, 1, 1);
    public string? Province { get; set; } = "Gauteng";
    public string? EmergencyContact { get; set; } = "0123456789";
}