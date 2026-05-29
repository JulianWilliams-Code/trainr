using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Trainr.Application.Auth.DTOs;
using Trainr.Application.Common;
using Trainr.Application.Common.Interfaces;
using Trainr.Domain.Entities;
using Trainr.Domain.Enums;
using Trainr.Infrastructure.Identity;
using Trainr.Infrastructure.Persistence;

namespace Trainr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly AppDbContext _db;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService,
        AppDbContext db)
    {
        _userManager   = userManager;
        _signInManager = signInManager;
        _jwtService    = jwtService;
        _db            = db;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<AuthResponse>.Fail("Invalid request.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

        var allowedRoles = new[] { UserRole.Client, UserRole.Trainer };
        if (!allowedRoles.Contains(request.Role))
            return BadRequest(ApiResponse<AuthResponse>.Fail("Role must be 'Client' or 'Trainer'."));

        if (await _userManager.FindByEmailAsync(request.Email) is not null)
            return Conflict(ApiResponse<AuthResponse>.Fail("Email is already registered."));

        var user = new ApplicationUser
        {
            UserName       = request.Email,
            Email          = request.Email,
            FirstName      = request.FirstName,
            LastName       = request.LastName,
            EmailConfirmed = true  // skip email confirmation for now
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(ApiResponse<AuthResponse>.Fail("Registration failed.",
                result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, request.Role);

        // Create matching profile
        if (request.Role == UserRole.Trainer)
            _db.TrainerProfiles.Add(new TrainerProfile { UserId = user.Id });
        else
            _db.ClientProfiles.Add(new ClientProfile { UserId = user.Id });

        await _db.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user.Id, user.Email!, user.FirstName, user.LastName, request.Role);

        return Ok(ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            Token     = token,
            Email     = user.Email!,
            FirstName = user.FirstName,
            LastName  = user.LastName,
            Role      = request.Role,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        }, "Registration successful."));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<AuthResponse>.Fail("Invalid request."));

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !user.IsActive)
            return Unauthorized(ApiResponse<AuthResponse>.Fail("Invalid credentials."));

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
            return Unauthorized(ApiResponse<AuthResponse>.Fail("Invalid credentials."));

        var roles = await _userManager.GetRolesAsync(user);
        var role  = roles.FirstOrDefault() ?? UserRole.Client;

        var token = _jwtService.GenerateToken(user.Id, user.Email!, user.FirstName, user.LastName, role);

        return Ok(ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            Token     = token,
            Email     = user.Email!,
            FirstName = user.FirstName,
            LastName  = user.LastName,
            Role      = role,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        }));
    }
}
