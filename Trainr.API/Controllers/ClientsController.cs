using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainr.Application.Clients.DTOs;
using Trainr.Application.Common;
using Trainr.Infrastructure.Identity;
using Trainr.Infrastructure.Persistence;

namespace Trainr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ClientsController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db          = db;
        _userManager = userManager;
    }

    // GET /api/clients/me
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<ClientProfileDto>>> GetMyProfile()
    {
        var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profile = await _db.ClientProfiles.FirstOrDefaultAsync(c => c.UserId == userId);

        if (profile is null)
            return NotFound(ApiResponse<ClientProfileDto>.Fail("Client profile not found."));

        var user = await _userManager.FindByIdAsync(userId);
        return Ok(ApiResponse<ClientProfileDto>.Ok(MapToDto(profile, user)));
    }

    // PUT /api/clients/me
    [HttpPut("me")]
    public async Task<ActionResult<ApiResponse<ClientProfileDto>>> UpdateMyProfile(
        UpdateClientProfileRequest request)
    {
        var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profile = await _db.ClientProfiles.FirstOrDefaultAsync(c => c.UserId == userId);

        if (profile is null)
            return NotFound(ApiResponse<ClientProfileDto>.Fail("Client profile not found."));

        if (request.FitnessGoals   is not null) profile.FitnessGoals   = request.FitnessGoals;
        if (request.PreferredSport is not null) profile.PreferredSport = request.PreferredSport;
        if (request.ProfileImageUrl is not null) profile.ProfileImageUrl = request.ProfileImageUrl;

        // Allow updating name on the identity user as well
        var user = await _userManager.FindByIdAsync(userId);
        if (user is not null)
        {
            if (request.FirstName is not null) user.FirstName = request.FirstName;
            if (request.LastName  is not null) user.LastName  = request.LastName;
            await _userManager.UpdateAsync(user);
        }

        await _db.SaveChangesAsync();

        return Ok(ApiResponse<ClientProfileDto>.Ok(MapToDto(profile, user), "Profile updated."));
    }

    // GET /api/clients/{id}  (Admin or Trainer only)
    [Authorize(Roles = "Admin,Trainer")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ClientProfileDto>>> GetById(int id)
    {
        var profile = await _db.ClientProfiles.FindAsync(id);
        if (profile is null)
            return NotFound(ApiResponse<ClientProfileDto>.Fail("Client not found."));

        var user = await _userManager.FindByIdAsync(profile.UserId);
        return Ok(ApiResponse<ClientProfileDto>.Ok(MapToDto(profile, user)));
    }

    private static ClientProfileDto MapToDto(Domain.Entities.ClientProfile profile, ApplicationUser? user) => new()
    {
        Id              = profile.Id,
        UserId          = profile.UserId,
        FirstName       = user?.FirstName     ?? string.Empty,
        LastName        = user?.LastName      ?? string.Empty,
        Email           = user?.Email         ?? string.Empty,
        FitnessGoals    = profile.FitnessGoals,
        PreferredSport  = profile.PreferredSport,
        ProfileImageUrl = profile.ProfileImageUrl
    };
}
