using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using InformationSystem.Domain.Models;

namespace Infrastructure.EntityConfiguration;

public class DebtPayableEntityTypeConfiguration : IEntityTypeConfiguration<DebtPayable>
{
    public void Configure(EntityTypeBuilder<DebtPayable> builder)
    {
        builder.HasKey(dp => dp.DebtPayableId);

        builder.Property(dp => dp.Amount).HasColumnType("decimal(18,2)");
        builder.Property(dp => dp.DueDate).HasColumnType("date");

        builder.HasOne(dp => dp.Expense)
            .WithMany(e => e.DebtPayables)
            .HasForeignKey(dp => dp.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}