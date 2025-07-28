using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using InformationSystem.Domain.Models;

namespace Infrastructure.EntityConfiguration;

public class DebtReceivableEntityTypeConfiguration : IEntityTypeConfiguration<DebtReceivable>
{
    public void Configure(EntityTypeBuilder<DebtReceivable> builder)
    {
        builder.HasKey(dr => dr.DebtReceivableId);

        builder.Property(dr => dr.Amount).HasColumnType("decimal(18,2)");
        builder.Property(dr => dr.DueDate).HasColumnType("date");

        builder.HasOne(dr => dr.Income)
            .WithMany(i => i.DebtReceivables)
            .HasForeignKey(dr => dr.IncomeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}