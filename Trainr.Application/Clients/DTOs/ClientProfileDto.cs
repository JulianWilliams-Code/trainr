using System.ComponentModel.DataAnnotations;

namespace Trainr.Application.Clients.DTOs;

public class ClientProfileDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FitnessGoals { get; set; }
    public string? PreferredSport { get; set; }
    public string? ProfileImageUrl { get; set; }
}

public class UpdateClientProfileRequest
{
    [MaxLength(1000)]
    public string? FitnessGoals { get; set; }

    [MaxLength(100)]
    public string? PreferredSport { get; set; }

    [MaxLength(500)]
    public string? ProfileImageUrl { get; set; }

    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }
}
