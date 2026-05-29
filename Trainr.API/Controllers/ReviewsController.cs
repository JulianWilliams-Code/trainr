using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainr.Application.Common;
using Trainr.Application.Reviews.DTOs;
using Trainr.Domain.Entities;
using Trainr.Domain.Enums;
using Trainr.Infrastructure.Identity;
using Trainr.Infrastructure.Persistence;

namespace Trainr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewsController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db          = db;
        _userManager = userManager;
    }

    // GET /api/reviews/trainer/{trainerId}
    [HttpGet("trainer/{trainerId:int}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReviewDto>>>> GetForTrainer(int trainerId)
    {
        var reviews = await _db.Reviews
            .Where(r => r.TrainerProfileId == trainerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var dtos = await Task.WhenAll(reviews.Select(BuildDto));
        return Ok(ApiResponse<IEnumerable<ReviewDto>>.Ok(dtos));
    }

    // POST /api/reviews  (client reviews a completed booking)
    [Authorize(Roles = "Client")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReviewDto>>> Create(CreateReviewRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var client = await _db.ClientProfiles.FirstOrDefaultAsync(c => c.UserId == userId);

        if (client is null)
            return NotFound(ApiResponse<ReviewDto>.Fail("Client profile not found."));

        var booking = await _db.Bookings.FindAsync(request.BookingId);
        if (booking is null)
            return NotFound(ApiResponse<ReviewDto>.Fail("Booking not found."));

        if (booking.ClientProfileId != client.Id)
            return Forbid();

        if (booking.Status != BookingStatus.Completed)
            return BadRequest(ApiResponse<ReviewDto>.Fail("You can only review completed sessions."));

        var alreadyReviewed = await _db.Reviews.AnyAsync(r => r.BookingId == request.BookingId);
        if (alreadyReviewed)
            return Conflict(ApiResponse<ReviewDto>.Fail("You have already reviewed this session."));

        var review = new Review
        {
            TrainerProfileId = booking.TrainerProfileId,
            ClientProfileId  = client.Id,
            BookingId        = booking.Id,
            Rating           = request.Rating,
            Comment          = request.Comment
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        // Update trainer's average rating
        await UpdateTrainerRating(booking.TrainerProfileId);

        return CreatedAtAction(nameof(GetForTrainer), new { trainerId = booking.TrainerProfileId },
            ApiResponse<ReviewDto>.Ok(await BuildDto(review), "Review submitted."));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task UpdateTrainerRating(int trainerProfileId)
    {
        var trainer = await _db.TrainerProfiles.FindAsync(trainerProfileId);
        if (trainer is null) return;

        var stats = await _db.Reviews
            .Where(r => r.TrainerProfileId == trainerProfileId)
            .GroupBy(_ => 1)
            .Select(g => new { Avg = g.Average(r => (double)r.Rating), Count = g.Count() })
            .FirstOrDefaultAsync();

        if (stats is not null)
        {
            trainer.AverageRating = Math.Round(stats.Avg, 2);
            trainer.TotalReviews  = stats.Count;
            await _db.SaveChangesAsync();
        }
    }

    private async Task<ReviewDto> BuildDto(Review review)
    {
        var clientProfile = await _db.ClientProfiles.FindAsync(review.ClientProfileId);
        var clientUser    = clientProfile is not null
            ? await _userManager.FindByIdAsync(clientProfile.UserId) : null;

        return new ReviewDto
        {
            Id               = review.Id,
            TrainerProfileId = review.TrainerProfileId,
            ClientFirstName  = clientUser?.FirstName ?? string.Empty,
            ClientLastName   = clientUser?.LastName  ?? string.Empty,
            Rating           = review.Rating,
            Comment          = review.Comment,
            CreatedAt        = review.CreatedAt
        };
    }
}
