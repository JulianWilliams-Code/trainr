namespace Trainr.Domain.Entities;

public class Availability : BaseEntity
{
    public int TrainerProfileId { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsBooked { get; set; }

    // Navigation
    public TrainerProfile TrainerProfile { get; set; } = null!;
}
