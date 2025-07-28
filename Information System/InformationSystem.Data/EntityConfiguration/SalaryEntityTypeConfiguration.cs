using InformationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityConfiguration;

public class SalaryEntityTypeConfiguration : IEntityTypeConfiguration<Salary>
{
    public void Configure(EntityTypeBuilder<Salary> builder)
    {
        builder.HasKey(s => s.SalaryId);
        builder.Property(s => s.Amount).HasColumnType("numeric(18,2)").IsRequired();
        builder.Property(s => s.PaymentDate).IsRequired().HasColumnType("date");
        
        builder.HasOne(s => s.Expense)
            .WithOne(e => e.Salary)
            .OnDelete(DeleteBehavior.Cascade);
    }
}