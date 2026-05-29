using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainr.Application.Admin.DTOs;
using Trainr.Application.Common;
using Trainr.Domain.Enums;
using Trainr.Infrastructure.Identity;
using Trainr.Infrastructure.Persistence;

namespace Trainr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db          = db;
        _userManager = userManager;
    }

    // GET /api/admin/stats
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<AdminStatsDto>>> GetStats()
    {
        var totalUsers    = await _userManager.Users.CountAsync();
        var trainerCount  = await _db.TrainerProfiles.CountAsync();
        var clientCount   = await _db.ClientProfiles.CountAsync();

        var bookingStats = await _db.Bookings
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total     = g.Count(),
                Pending   = g.Count(b => b.Status == Domain.Enums.BookingStatus.Pending),
                Completed = g.Count(b => b.Status == Domain.Enums.BookingStatus.Completed),
                Cancelled = g.Count(b => b.Status == Domain.Enums.BookingStatus.Cancelled),
                Revenue   = g.Where(b => b.Status == Domain.Enums.BookingStatus.Completed)
                             .Sum(b => b.TotalPrice)
            })
            .FirstOrDefaultAsync();

        var unverified = await _db.TrainerProfiles.CountAsync(t => !t.IsVerified);

        return Ok(ApiResponse<AdminStatsDto>.Ok(new AdminStatsDto
        {
            TotalUsers         = totalUsers,
            TotalTrainers      = trainerCount,
            TotalClients       = clientCount,
            TotalBookings      = bookingStats?.Total     ?? 0,
            PendingBookings    = bookingStats?.Pending   ?? 0,
            CompletedBookings  = bookingStats?.Completed ?? 0,
            CancelledBookings  = bookingStats?.Cancelled ?? 0,
            TotalRevenue       = bookingStats?.Revenue   ?? 0,
            UnverifiedTrainers = unverified
        }));
    }

    // GET /api/admin/users?page=1&pageSize=20
    [HttpGet("users")]
    public async Task<ActionResult<ApiResponse<PagedResult<AdminUserDto>>>> GetUsers(
        int page = 1, int pageSize = 20)
    {
        var users = await _userManager.Users
            .OrderBy(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalCount = await _userManager.Users.CountAsync();

        var dtos = new List<AdminUserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            dtos.Add(new AdminUserDto
            {
                Id        = user.Id,
                FirstName = user.FirstName,
                LastName  = user.LastName,
                Email     = user.Email ?? string.Empty,
                Role      = roles.FirstOrDefault() ?? string.Empty,
                IsActive  = user.IsActive,
                CreatedAt = user.CreatedAt
            });
        }

        return Ok(ApiResponse<PagedResult<AdminUserDto>>.Ok(new PagedResult<AdminUserDto>
        {
            Items      = dtos,
            TotalCount = totalCount,
            Page       = page,
            PageSize   = pageSize
        }));
    }

    // PATCH /api/admin/users/{id}/status
    [HttpPatch("users/{id}/status")]
    public async Task<ActionResult<ApiResponse<object>>> SetUserStatus(string id, bool isActive)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound(ApiResponse<object>.Fail("User not found."));

        // Prevent deactivating yourself
        if (user.Id == User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
            return BadRequest(ApiResponse<object>.Fail("You cannot deactivate your own account."));

        user.IsActive = isActive;
        await _userManager.UpdateAsync(user);

        return Ok(ApiResponse<object>.Ok(new { }, $"User {(isActive ? "activated" : "deactivated")}."));
    }

    // GET /api/admin/trainers?verified=false&page=1
    [HttpGet("trainers")]
    public async Task<ActionResult<ApiResponse<PagedResult<AdminTrainerDto>>>> GetTrainers(
        bool? verified = null, int page = 1, int pageSize = 20)
    {
        var query = _db.TrainerProfiles.AsQueryable();

        if (verified.HasValue)
            query = query.Where(t => t.IsVerified == verified.Value);

        var totalCount = await query.CountAsync();

        var profiles = await query
            .OrderBy(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = new List<AdminTrainerDto>();
        foreach (var profile in profiles)
        {
            var user = await _userManager.FindByIdAsync(profile.UserId);
            dtos.Add(new AdminTrainerDto
            {
                ProfileId     = profile.Id,
                UserId        = profile.UserId,
                FirstName     = user?.FirstName ?? string.Empty,
                LastName      = user?.LastName  ?? string.Empty,
                Email         = user?.Email     ?? string.Empty,
                SportType     = profile.SportType,
                HourlyRate    = profile.HourlyRate,
                City          = profile.City,
                State         = profile.State,
                AverageRating = profile.AverageRating,
                TotalReviews  = profile.TotalReviews,
                IsVerified    = profile.IsVerified,
                IsActive      = profile.IsActive,
                CreatedAt     = profile.CreatedAt
            });
        }

        return Ok(ApiResponse<PagedResult<AdminTrainerDto>>.Ok(new PagedResult<AdminTrainerDto>
        {
            Items      = dtos,
            TotalCount = totalCount,
            Page       = page,
            PageSize   = pageSize
        }));
    }

    // PATCH /api/admin/trainers/{id}/verify
    [HttpPatch("trainers/{id:int}/verify")]
    public async Task<ActionResult<ApiResponse<object>>> VerifyTrainer(int id, bool isVerified)
    {
        var profile = await _db.TrainerProfiles.FindAsync(id);
        if (profile is null)
            return NotFound(ApiResponse<object>.Fail("Trainer profile not found."));

        profile.IsVerified = isVerified;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(new { }, $"Trainer {(isVerified ? "verified" : "unverified")}."));
    }

    // DELETE /api/admin/users/{id}  (soft delete)
    [HttpDelete("users/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound(ApiResponse<object>.Fail("User not found."));

        if (user.Id == User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
            return BadRequest(ApiResponse<object>.Fail("You cannot delete your own account."));

        // Soft delete — deactivate rather than hard delete
        user.IsActive = false;
        await _userManager.UpdateAsync(user);

        // Also soft-delete the matching profile
        var trainerProfile = await _db.TrainerProfiles.FirstOrDefaultAsync(t => t.UserId == id);
        if (trainerProfile is not null) trainerProfile.IsActive = false;

        var clientProfile = await _db.ClientProfiles.FirstOrDefaultAsync(c => c.UserId == id);
        if (clientProfile is not null) clientProfile.IsActive = false;

        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(new { }, "User deactivated."));
    }
}
