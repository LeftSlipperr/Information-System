using InformationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityConfiguration;

public class RawMaterialWarehouseEntityTypeConfiguration : IEntityTypeConfiguration<RawMaterialWarehouse>
{
    public void Configure(EntityTypeBuilder<RawMaterialWarehouse> builder)
    {
        builder.HasKey(rmw => rmw.MaterialId);

        builder.Property(rmw => rmw.PurchaseCost).HasColumnType("decimal(18,2)");
        builder.Property(rmw => rmw.LastUpdated).HasColumnType("date");

        builder.HasOne(rmw => rmw.Expense)
            .WithMany(e => e.RawMaterialWarehouses)
            .HasForeignKey(rmw => rmw.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}