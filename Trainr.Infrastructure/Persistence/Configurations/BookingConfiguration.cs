using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trainr.Domain.Entities;
using Trainr.Domain.Enums;

namespace Trainr.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.TotalPrice).HasColumnType("decimal(10,2)");
        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(b => b.Notes).HasMaxLength(1000);

        builder.HasOne(b => b.TrainerProfile)
            .WithMany(t => t.Bookings)
            .HasForeignKey(b => b.TrainerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.ClientProfile)
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.ClientProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Availability)
            .WithMany()
            .HasForeignKey(b => b.AvailabilityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => new { b.TrainerProfileId, b.Status });
        builder.HasIndex(b => new { b.ClientProfileId, b.Status });

        builder.HasQueryFilter(b => b.IsActive);
    }
}
