using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trainr.Domain.Entities;

namespace Trainr.Infrastructure.Persistence.Configurations;

public class ClientProfileConfiguration : IEntityTypeConfiguration<ClientProfile>
{
    public void Configure(EntityTypeBuilder<ClientProfile> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId).IsRequired();
        builder.Property(c => c.FitnessGoals).HasMaxLength(1000);
        builder.Property(c => c.PreferredSport).HasMaxLength(100);
        builder.Property(c => c.ProfileImageUrl).HasMaxLength(500);

        builder.HasIndex(c => c.UserId).IsUnique();

        builder.HasQueryFilter(c => c.IsActive);
    }
}
