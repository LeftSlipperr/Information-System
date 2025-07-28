using InformationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfiguration;

public class IncomeEntityTypeConfiguration : IEntityTypeConfiguration<Income>
{
    public void Configure(EntityTypeBuilder<Income> builder)
    {
        builder.HasKey(i => i.IncomeId);

        builder.Property(i => i.Amount).HasColumnType("decimal(18,2)");
        builder.Property(i => i.Source).HasMaxLength(100);
        builder.Property(i => i.Date).HasColumnType("date");

        builder.HasOne(i => i.BalanceAnalysis).WithMany(b => b.Incomes);
    }
}
