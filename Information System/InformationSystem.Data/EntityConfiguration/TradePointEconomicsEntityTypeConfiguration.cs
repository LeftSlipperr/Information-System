using InformationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityConfiguration;

public class TradePointEconomicsEntityTypeConfiguration : IEntityTypeConfiguration<TradePointEconomics>
{
    public void Configure(EntityTypeBuilder<TradePointEconomics> builder)
    {
        builder.HasKey(tpe => tpe.TradePointEconomicsId);

        builder.Property(tpe => tpe.Revenue).HasColumnType("decimal(18,2)");
        builder.Property(tpe => tpe.Expenses).HasColumnType("decimal(18,2)");
        builder.Property(tpe => tpe.Profit).HasColumnType("decimal(18,2)");
        builder.Property(tpe => tpe.Period).HasColumnType("date");

        builder.HasOne(t => t.TradePoint).WithMany();
        
        builder.HasOne(tpe => tpe.Income)
            .WithOne(i => i.TradePointEconomics)
            .HasForeignKey<TradePointEconomics>(tpe => tpe.IncomeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tpe => tpe.Expense)
            .WithOne(e => e.TradePointEconomics)
            .HasForeignKey<TradePointEconomics>(tpe => tpe.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}