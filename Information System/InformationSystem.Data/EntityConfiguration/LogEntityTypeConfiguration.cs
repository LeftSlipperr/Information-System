using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using InformationSystem.Domain.Models;

namespace Infrastructure.EntityConfiguration;

public class LogEntityTypeConfiguration : IEntityTypeConfiguration<Log>
{
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.HasKey(l => l.LogId);
        builder.Property(l => l.Timestamp).HasColumnType("timestamp");

        builder.HasOne(l => l.User).WithMany(u => u.Logs);

    }
}