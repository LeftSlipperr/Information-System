using InformationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfiguration;

public class WorkerEntityTypeConfiguration : IEntityTypeConfiguration<Worker>
{
    public void Configure(EntityTypeBuilder<Worker> builder)
    {
        builder.HasKey(w => w.WorkerId);

        builder.Property(w => w.Position).IsRequired().HasMaxLength(100);
        builder.Property(w => w.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(w => w.SecondName).IsRequired().HasMaxLength(50);
        builder.Property(w => w.ThirdName).HasMaxLength(50);
        
        builder.HasOne(w => w.Salary)
            .WithOne(s => s.Worker)
            .OnDelete(DeleteBehavior.Cascade);
    }
}