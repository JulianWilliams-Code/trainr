using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainr.Application.Common;
using Trainr.Application.Trainers.DTOs;
using Trainr.Domain.Entities;
using Trainr.Infrastructure.Identity;
using Trainr.Infrastructure.Persistence;

namespace Trainr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrainersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public TrainersController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db          = db;
        _userManager = userManager;
    }

    // GET /api/trainers/me  (trainer views their own full profile)
    [Authorize(Roles = "Trainer")]
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<TrainerProfileDto>>> GetMyProfile()
    {
        var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profile = await _db.TrainerProfiles.FirstOrDefaultAsync(t => t.UserId == userId);

        if (profile is null)
            return NotFound(ApiResponse<TrainerProfileDto>.Fail("Trainer profile not found."));

        var user = await _userManager.FindByIdAsync(userId);
        return Ok(ApiResponse<TrainerProfileDto>.Ok(MapToDto(profile, user)));
    }

    // GET /api/trainers?sportType=Soccer&city=Austin&page=1&pageSize=20
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<TrainerProfileDto>>>> Search(
        [FromQuery] TrainerSearchRequest request)
    {
        var query = _db.TrainerProfiles.Include(t => t.Availabilities).AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SportType))
            query = query.Where(t => t.SportType == request.SportType);

        if (!string.IsNullOrWhiteSpace(request.City))
            query = query.Where(t => t.City == request.City);

        if (!string.IsNullOrWhiteSpace(request.State))
            query = query.Where(t => t.State == request.State);

        if (request.MinRate.HasValue)
            query = query.Where(t => t.HourlyRate >= request.MinRate.Value);

        if (request.MaxRate.HasValue)
            query = query.Where(t => t.HourlyRate <= request.MaxRate.Value);

        if (request.MinRating.HasValue)
            query = query.Where(t => t.AverageRating >= request.MinRating.Value);

        var totalCount = await query.CountAsync();

        var profiles = await query
            .OrderByDescending(t => t.AverageRating)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var userIds = profiles.Select(p => p.UserId).ToList();
        var users   = await _userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id);

        var dtos = profiles.Select(p => MapToDto(p, users.GetValueOrDefault(p.UserId))).ToList();

        return Ok(ApiResponse<PagedResult<TrainerProfileDto>>.Ok(new PagedResult<TrainerProfileDto>
        {
            Items      = dtos,
            TotalCount = totalCount,
            Page       = request.Page,
            PageSize   = request.PageSize
        }));
    }

    // GET /api/trainers/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<TrainerProfileDto>>> GetById(int id)
    {
        var profile = await _db.TrainerProfiles.FindAsync(id);
        if (profile is null)
            return NotFound(ApiResponse<TrainerProfileDto>.Fail("Trainer not found."));

        var user = await _userManager.FindByIdAsync(profile.UserId);
        return Ok(ApiResponse<TrainerProfileDto>.Ok(MapToDto(profile, user)));
    }

    // PUT /api/trainers/profile  (trainer updates their own profile)
    [Authorize(Roles = "Trainer")]
    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse<TrainerProfileDto>>> UpdateProfile(
        UpdateTrainerProfileRequest request)
    {
        var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profile = await _db.TrainerProfiles.FirstOrDefaultAsync(t => t.UserId == userId);

        if (profile is null)
            return NotFound(ApiResponse<TrainerProfileDto>.Fail("Profile not found."));

        if (request.Bio                  is not null) profile.Bio                  = request.Bio;
        if (request.SportType            is not null) profile.SportType            = request.SportType;
        if (request.HourlyRate           is not null) profile.HourlyRate           = request.HourlyRate.Value;
        if (request.City                 is not null) profile.City                 = request.City;
        if (request.State                is not null) profile.State                = request.State;
        if (request.ProfileImageUrl      is not null) profile.ProfileImageUrl      = request.ProfileImageUrl;
        if (request.CertificationDetails is not null) profile.CertificationDetails = request.CertificationDetails;
        if (request.YearsOfExperience    is not null) profile.YearsOfExperience   = request.YearsOfExperience.Value;

        await _db.SaveChangesAsync();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is not null)
        {
            if (request.FirstName is not null) user.FirstName = request.FirstName;
            if (request.LastName  is not null) user.LastName  = request.LastName;
            await _userManager.UpdateAsync(user);
        }

        return Ok(ApiResponse<TrainerProfileDto>.Ok(MapToDto(profile, user), "Profile updated."));
    }

    // GET /api/trainers/{id}/availability
    [HttpGet("{id:int}/availability")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AvailabilityDto>>>> GetAvailability(int id)
    {
        var exists = await _db.TrainerProfiles.AnyAsync(t => t.Id == id);
        if (!exists)
            return NotFound(ApiResponse<IEnumerable<AvailabilityDto>>.Fail("Trainer not found."));

        var slots = await _db.Availabilities
            .Where(a => a.TrainerProfileId == id && !a.IsBooked && a.StartTime > DateTime.UtcNow)
            .OrderBy(a => a.StartTime)
            .Select(a => new AvailabilityDto
            {
                Id        = a.Id,
                StartTime = a.StartTime,
                EndTime   = a.EndTime,
                IsBooked  = a.IsBooked
            })
            .ToListAsync();

        return Ok(ApiResponse<IEnumerable<AvailabilityDto>>.Ok(slots));
    }

    // POST /api/trainers/availability  (trainer adds their own open slots)
    [Authorize(Roles = "Trainer")]
    [HttpPost("availability")]
    public async Task<ActionResult<ApiResponse<AvailabilityDto>>> AddAvailability(
        AddAvailabilityRequest request)
    {
        if (request.EndTime <= request.StartTime)
            return BadRequest(ApiResponse<AvailabilityDto>.Fail("End time must be after start time."));

        if (request.StartTime < DateTime.UtcNow)
            return BadRequest(ApiResponse<AvailabilityDto>.Fail("Start time must be in the future."));

        var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profile = await _db.TrainerProfiles.FirstOrDefaultAsync(t => t.UserId == userId);

        if (profile is null)
            return NotFound(ApiResponse<AvailabilityDto>.Fail("Trainer profile not found."));

        var slot = new Availability
        {
            TrainerProfileId = profile.Id,
            StartTime        = request.StartTime,
            EndTime          = request.EndTime
        };

        _db.Availabilities.Add(slot);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAvailability), new { id = profile.Id },
            ApiResponse<AvailabilityDto>.Ok(new AvailabilityDto
            {
                Id        = slot.Id,
                StartTime = slot.StartTime,
                EndTime   = slot.EndTime,
                IsBooked  = slot.IsBooked
            }, "Availability slot added."));
    }

    private static TrainerProfileDto MapToDto(TrainerProfile profile, ApplicationUser? user) => new()
    {
        Id                   = profile.Id,
        UserId               = profile.UserId,
        FirstName            = user?.FirstName ?? string.Empty,
        LastName             = user?.LastName  ?? string.Empty,
        Email                = user?.Email     ?? string.Empty,
        Bio                  = profile.Bio,
        SportType            = profile.SportType,
        HourlyRate           = profile.HourlyRate,
        City                 = profile.City,
        State                = profile.State,
        AverageRating        = profile.AverageRating,
        TotalReviews         = profile.TotalReviews,
        IsVerified           = profile.IsVerified,
        ProfileImageUrl      = profile.ProfileImageUrl,
        CertificationDetails = profile.CertificationDetails,
        YearsOfExperience    = profile.YearsOfExperience
    };
}
