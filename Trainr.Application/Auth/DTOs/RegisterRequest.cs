using System.ComponentModel.DataAnnotations;

namespace Trainr.Application.Auth.DTOs;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    /// <summary>Must be "Client" or "Trainer"</summary>
    [Required]
    public string Role { get; set; } = string.Empty;
}
