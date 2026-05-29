using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Trainr.Domain.Entities;
using Trainr.Infrastructure.Identity;
using Trainr.Infrastructure.Persistence.Configurations;

namespace Trainr.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TrainerProfile> TrainerProfiles => Set<TrainerProfile>();
    public DbSet<ClientProfile>  ClientProfiles  => Set<ClientProfile>();
    public DbSet<Availability>   Availabilities  => Set<Availability>();
    public DbSet<Booking>        Bookings        => Set<Booking>();
    public DbSet<Review>         Reviews         => Set<Review>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new TrainerProfileConfiguration());
        builder.ApplyConfiguration(new ClientProfileConfiguration());
        builder.ApplyConfiguration(new AvailabilityConfiguration());
        builder.ApplyConfiguration(new BookingConfiguration());
        builder.ApplyConfiguration(new ReviewConfiguration());
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<Domain.Entities.BaseEntity>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
            entry.Entity.UpdatedAt = DateTime.UtcNow;
    }
}
