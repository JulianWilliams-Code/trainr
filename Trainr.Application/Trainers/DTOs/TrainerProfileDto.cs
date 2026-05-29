namespace Trainr.Application.Trainers.DTOs;

public class TrainerProfileDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string SportType { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public bool IsVerified { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? CertificationDetails { get; set; }
    public int YearsOfExperience { get; set; }
}
