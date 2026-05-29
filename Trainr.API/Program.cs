using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Trainr.API.Filters;
using Trainr.API.Middleware;
using Trainr.Application.Common.Interfaces;
using Trainr.Infrastructure.Identity;
using Trainr.Infrastructure.Persistence;
using Trainr.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────────────
// Dev: SQLite  |  Prod: SQL Server (set ASPNETCORE_ENVIRONMENT=Production)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

if (builder.Environment.IsProduction())
    builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(connectionString));
else
    builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite(connectionString));

// ── Identity ──────────────────────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit           = true;
    options.Password.RequiredLength         = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase       = false;
    options.User.RequireUniqueEmail         = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ── JWT Authentication ────────────────────────────────────────────────────────
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey         = new SymmetricSecurityKey(
                                       Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ValidateIssuer   = true,
        ValidIssuer      = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience    = builder.Configuration["Jwt:Audience"],
        ClockSkew        = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ── CORS ──────────────────────────────────────────────────────────────────────
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                     ?? ["http://localhost:3000"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("TrainrPolicy", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// ── Rate Limiting ─────────────────────────────────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    // Auth endpoints: 10 requests / minute per IP
    options.AddFixedWindowLimiter("auth", o =>
    {
        o.Window            = TimeSpan.FromMinutes(1);
        o.PermitLimit       = 10;
        o.QueueLimit        = 0;
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // General API: 120 requests / minute per IP
    options.AddFixedWindowLimiter("api", o =>
    {
        o.Window            = TimeSpan.FromMinutes(1);
        o.PermitLimit       = 120;
        o.QueueLimit        = 0;
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// ── Application Services ──────────────────────────────────────────────────────
builder.Services.AddScoped<IJwtService, JwtService>();

// ── Health Checks ─────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

// ── API + Swagger ─────────────────────────────────────────────────────────────
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
})
.ConfigureApiBehaviorOptions(options =>
{
    // Disable automatic 400 — our ValidationFilter handles it
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "Trainr Marketplace API",
        Version     = "v1",
        Description = "REST API for the Trainr trainer marketplace — find, book, and review personal trainers."
    });

    // Add JWT bearer auth to Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Enter your JWT token. Example: eyJhbGci..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            []
        }
    });
});

var app = builder.Build();

// ── Seed Database ─────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    await DbSeeder.SeedRolesAsync(scope.ServiceProvider);
    await DbSeeder.SeedAdminAsync(
        scope.ServiceProvider,
        app.Configuration["AdminSeed:Email"]!,
        app.Configuration["AdminSeed:Password"]!);
}

// ── Middleware Pipeline ───────────────────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Trainr API v1");
        options.RoutePrefix = "docs";   // → http://localhost:PORT/docs
    });
}

app.UseCors("TrainrPolicy");
app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
