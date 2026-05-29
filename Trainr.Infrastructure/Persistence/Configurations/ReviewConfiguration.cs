using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trainr.Domain.Entities;

namespace Trainr.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Comment).IsRequired().HasMaxLength(2000);
        builder.Property(r => r.Rating).IsRequired();

        builder.HasOne(r => r.TrainerProfile)
            .WithMany(t => t.Reviews)
            .HasForeignKey(r => r.TrainerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ClientProfile)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.ClientProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        // One review per booking
        builder.HasIndex(r => r.BookingId).IsUnique();

        builder.HasQueryFilter(r => r.IsActive);
    }
}
