using System.ComponentModel.DataAnnotations;

namespace Trainr.Application.Trainers.DTOs;

public class UpdateTrainerProfileRequest
{
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [MaxLength(2000)]
    public string? Bio { get; set; }

    [MaxLength(100)]
    public string? SportType { get; set; }

    [Range(0, 10000)]
    public decimal? HourlyRate { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(500)]
    public string? ProfileImageUrl { get; set; }

    [MaxLength(1000)]
    public string? CertificationDetails { get; set; }

    [Range(0, 80)]
    public int? YearsOfExperience { get; set; }
}
