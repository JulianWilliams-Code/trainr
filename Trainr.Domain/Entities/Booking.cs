using Trainr.Domain.Enums;

namespace Trainr.Domain.Entities;

public class Booking : BaseEntity
{
    public int TrainerProfileId { get; set; }
    public int ClientProfileId { get; set; }
    public int AvailabilityId { get; set; }

    public DateTime SessionStart { get; set; }
    public DateTime SessionEnd { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public TrainerProfile TrainerProfile { get; set; } = null!;
    public ClientProfile ClientProfile { get; set; } = null!;
    public Availability Availability { get; set; } = null!;
}
