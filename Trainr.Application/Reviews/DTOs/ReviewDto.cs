using System.ComponentModel.DataAnnotations;

namespace Trainr.Application.Reviews.DTOs;

public class ReviewDto
{
    public int Id { get; set; }
    public int TrainerProfileId { get; set; }
    public string ClientFirstName { get; set; } = string.Empty;
    public string ClientLastName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewRequest
{
    public int BookingId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Comment { get; set; } = string.Empty;
}
