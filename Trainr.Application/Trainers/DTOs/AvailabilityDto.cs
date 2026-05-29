namespace Trainr.Application.Trainers.DTOs;

public class AvailabilityDto
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsBooked { get; set; }
}

public class AddAvailabilityRequest
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
