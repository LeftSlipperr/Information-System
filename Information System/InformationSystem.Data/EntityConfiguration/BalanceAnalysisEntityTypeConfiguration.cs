using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using InformationSystem.Domain.Models;

namespace Infrastructure.EntityConfiguration;

public class BalanceAnalysisEntityTypeConfiguration : IEntityTypeConfiguration<BalanceAnalysis>
{
    public void Configure(EntityTypeBuilder<BalanceAnalysis> builder)
    {
        builder.HasKey(ba => ba.BalanceId);

        builder.Property(ba => ba.TotalIncome).HasColumnType("decimal(18,2)");
        builder.Property(ba => ba.TotalExpense).HasColumnType("decimal(18,2)");
        builder.Property(ba => ba.ProfitOrLoss).HasColumnType("decimal(18,2)");
        builder.Property(ba => ba.Period).HasColumnType("date");

        builder.HasMany(b => b.Incomes).WithOne(i => i.BalanceAnalysis);
        builder.HasMany(b => b.Expenses).WithOne(e => e.BalanceAnalysis);
    }
}