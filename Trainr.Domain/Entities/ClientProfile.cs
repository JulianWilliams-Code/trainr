namespace Trainr.Domain.Entities;

public class ClientProfile : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

    public string? FitnessGoals { get; set; }
    public string? PreferredSport { get; set; }
    public string? ProfileImageUrl { get; set; }

    // Navigation
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
