namespace Trainr.Domain.Entities;

public class Review : BaseEntity
{
    public int TrainerProfileId { get; set; }
    public int ClientProfileId { get; set; }
    public int BookingId { get; set; }

    public int Rating { get; set; }   // 1–5
    public string Comment { get; set; } = string.Empty;

    // Navigation
    public TrainerProfile TrainerProfile { get; set; } = null!;
    public ClientProfile ClientProfile { get; set; } = null!;
}
