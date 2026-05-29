namespace Trainr.Application.Trainers.DTOs;

public class TrainerSearchRequest
{
    public string? SportType { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public decimal? MinRate { get; set; }
    public decimal? MaxRate { get; set; }
    public double? MinRating { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
