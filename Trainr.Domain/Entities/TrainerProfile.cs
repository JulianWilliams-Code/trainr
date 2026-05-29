namespace Trainr.Domain.Entities;

public class TrainerProfile : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

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

    // Navigation
    public ICollection<Availability> Availabilities { get; set; } = new List<Availability>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
