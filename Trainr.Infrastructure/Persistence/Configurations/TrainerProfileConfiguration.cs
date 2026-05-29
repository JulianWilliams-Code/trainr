using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trainr.Domain.Entities;

namespace Trainr.Infrastructure.Persistence.Configurations;

public class TrainerProfileConfiguration : IEntityTypeConfiguration<TrainerProfile>
{
    public void Configure(EntityTypeBuilder<TrainerProfile> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.UserId).IsRequired();
        builder.Property(t => t.SportType).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Bio).HasMaxLength(2000);
        builder.Property(t => t.City).HasMaxLength(100);
        builder.Property(t => t.State).HasMaxLength(100);
        builder.Property(t => t.HourlyRate).HasColumnType("decimal(10,2)");
        builder.Property(t => t.ProfileImageUrl).HasMaxLength(500);
        builder.Property(t => t.CertificationDetails).HasMaxLength(1000);

        builder.HasIndex(t => t.UserId).IsUnique();
        builder.HasIndex(t => t.SportType);

        builder.HasQueryFilter(t => t.IsActive);
    }
}
