namespace Trainr.Application.Bookings.DTOs;

public class BookingDto
{
    public int Id { get; set; }
    public int TrainerProfileId { get; set; }
    public string TrainerFirstName { get; set; } = string.Empty;
    public string TrainerLastName { get; set; } = string.Empty;
    public int ClientProfileId { get; set; }
    public string ClientFirstName { get; set; } = string.Empty;
    public string ClientLastName { get; set; } = string.Empty;
    public DateTime SessionStart { get; set; }
    public DateTime SessionEnd { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateBookingRequest
{
    public int AvailabilityId { get; set; }
    public string? Notes { get; set; }
}

public class UpdateBookingStatusRequest
{
    /// <summary>Confirmed, Cancelled, Completed</summary>
    public string Status { get; set; } = string.Empty;
}
