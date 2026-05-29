using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trainr.Domain.Entities;

namespace Trainr.Infrastructure.Persistence.Configurations;

public class AvailabilityConfiguration : IEntityTypeConfiguration<Availability>
{
    public void Configure(EntityTypeBuilder<Availability> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.TrainerProfile)
            .WithMany(t => t.Availabilities)
            .HasForeignKey(a => a.TrainerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => new { a.TrainerProfileId, a.StartTime });

        builder.HasQueryFilter(a => a.IsActive);
    }
}
