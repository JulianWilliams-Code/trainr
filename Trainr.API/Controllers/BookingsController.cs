using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainr.Application.Bookings.DTOs;
using Trainr.Application.Common;
using Trainr.Domain.Entities;
using Trainr.Domain.Enums;
using Trainr.Infrastructure.Identity;
using Trainr.Infrastructure.Persistence;

namespace Trainr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookingsController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db          = db;
        _userManager = userManager;
    }

    // POST /api/bookings  (client books a slot)
    [Authorize(Roles = "Client")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<BookingDto>>> Create(CreateBookingRequest request)
    {
        var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var client  = await _db.ClientProfiles.FirstOrDefaultAsync(c => c.UserId == userId);

        if (client is null)
            return NotFound(ApiResponse<BookingDto>.Fail("Client profile not found."));

        var slot = await _db.Availabilities
            .Include(a => a.TrainerProfile)
            .FirstOrDefaultAsync(a => a.Id == request.AvailabilityId);

        if (slot is null)
            return NotFound(ApiResponse<BookingDto>.Fail("Availability slot not found."));

        if (slot.IsBooked)
            return Conflict(ApiResponse<BookingDto>.Fail("This slot is already booked."));

        var booking = new Booking
        {
            TrainerProfileId = slot.TrainerProfileId,
            ClientProfileId  = client.Id,
            AvailabilityId   = slot.Id,
            SessionStart     = slot.StartTime,
            SessionEnd       = slot.EndTime,
            TotalPrice       = slot.TrainerProfile.HourlyRate *
                               (decimal)(slot.EndTime - slot.StartTime).TotalHours,
            Status           = BookingStatus.Pending,
            Notes            = request.Notes
        };

        slot.IsBooked = true;

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = booking.Id },
            ApiResponse<BookingDto>.Ok(await BuildDto(booking), "Booking created."));
    }

    // GET /api/bookings/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<BookingDto>>> GetById(int id)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking is null)
            return NotFound(ApiResponse<BookingDto>.Fail("Booking not found."));

        if (!await CanAccessBooking(booking))
            return Forbid();

        return Ok(ApiResponse<BookingDto>.Ok(await BuildDto(booking)));
    }

    // GET /api/bookings/my  (returns bookings for whoever is calling)
    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<IEnumerable<BookingDto>>>> GetMine()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var role   = User.FindFirstValue(ClaimTypes.Role) ?? UserRole.Client;

        List<Booking> bookings;

        if (role == UserRole.Trainer)
        {
            var profile = await _db.TrainerProfiles.FirstOrDefaultAsync(t => t.UserId == userId);
            if (profile is null) return Ok(ApiResponse<IEnumerable<BookingDto>>.Ok([]));

            bookings = await _db.Bookings
                .Where(b => b.TrainerProfileId == profile.Id)
                .OrderByDescending(b => b.SessionStart)
                .ToListAsync();
        }
        else
        {
            var profile = await _db.ClientProfiles.FirstOrDefaultAsync(c => c.UserId == userId);
            if (profile is null) return Ok(ApiResponse<IEnumerable<BookingDto>>.Ok([]));

            bookings = await _db.Bookings
                .Where(b => b.ClientProfileId == profile.Id)
                .OrderByDescending(b => b.SessionStart)
                .ToListAsync();
        }

        var dtos = await Task.WhenAll(bookings.Select(BuildDto));
        return Ok(ApiResponse<IEnumerable<BookingDto>>.Ok(dtos));
    }

    // PATCH /api/bookings/{id}/status  (trainer confirms/cancels; client cancels)
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<ApiResponse<BookingDto>>> UpdateStatus(
        int id, UpdateBookingStatusRequest request)
    {
        var booking = await _db.Bookings
            .Include(b => b.Availability)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking is null)
            return NotFound(ApiResponse<BookingDto>.Fail("Booking not found."));

        if (!await CanAccessBooking(booking))
            return Forbid();

        if (!Enum.TryParse<BookingStatus>(request.Status, ignoreCase: true, out var newStatus))
            return BadRequest(ApiResponse<BookingDto>.Fail(
                "Invalid status. Use: Confirmed, Cancelled, Completed."));

        // If cancelling, free up the slot
        if (newStatus == BookingStatus.Cancelled)
            booking.Availability.IsBooked = false;

        booking.Status = newStatus;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<BookingDto>.Ok(await BuildDto(booking), "Status updated."));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task<bool> CanAccessBooking(Booking booking)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var role   = User.FindFirstValue(ClaimTypes.Role) ?? UserRole.Client;

        if (role == UserRole.Admin) return true;

        if (role == UserRole.Trainer)
        {
            var profile = await _db.TrainerProfiles.FirstOrDefaultAsync(t => t.UserId == userId);
            return profile is not null && booking.TrainerProfileId == profile.Id;
        }

        var clientProfile = await _db.ClientProfiles.FirstOrDefaultAsync(c => c.UserId == userId);
        return clientProfile is not null && booking.ClientProfileId == clientProfile.Id;
    }

    private async Task<BookingDto> BuildDto(Booking booking)
    {
        var trainerProfile = await _db.TrainerProfiles.FindAsync(booking.TrainerProfileId);
        var clientProfile  = await _db.ClientProfiles.FindAsync(booking.ClientProfileId);

        var trainerUser = trainerProfile is not null
            ? await _userManager.FindByIdAsync(trainerProfile.UserId) : null;
        var clientUser = clientProfile is not null
            ? await _userManager.FindByIdAsync(clientProfile.UserId) : null;

        return new BookingDto
        {
            Id               = booking.Id,
            TrainerProfileId = booking.TrainerProfileId,
            TrainerFirstName = trainerUser?.FirstName ?? string.Empty,
            TrainerLastName  = trainerUser?.LastName  ?? string.Empty,
            ClientProfileId  = booking.ClientProfileId,
            ClientFirstName  = clientUser?.FirstName  ?? string.Empty,
            ClientLastName   = clientUser?.LastName   ?? string.Empty,
            SessionStart     = booking.SessionStart,
            SessionEnd       = booking.SessionEnd,
            Status           = booking.Status.ToString(),
            TotalPrice       = booking.TotalPrice,
            Notes            = booking.Notes,
            CreatedAt        = booking.CreatedAt
        };
    }
}
