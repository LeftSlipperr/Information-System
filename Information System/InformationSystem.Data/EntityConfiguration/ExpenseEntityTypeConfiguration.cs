using InformationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfiguration;

public class ExpenseEntityTypeConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses");
        builder.HasKey(e => e.ExpenseId);

        builder.Property(e => e.ExpenseName).IsRequired();
        builder.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(e => e.Category).IsRequired();
        builder.Property(e => e.Date).IsRequired().HasColumnType("date");

        builder.HasOne(e => e.BalanceAnalysis).WithMany(b => b.Expenses);
    }
}