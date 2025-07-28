using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using InformationSystem.Domain.Models;

namespace Infrastructure.EntityConfiguration;

public class FinishedGoodsWarehouseEntityTypeConfiguration : IEntityTypeConfiguration<FinishedGoodsWarehouse>
{
    public void Configure(EntityTypeBuilder<FinishedGoodsWarehouse> builder)
    {
        builder.HasKey(fgw => fgw.ProductId);

        builder.Property(fgw => fgw.PotentialRevenue).HasColumnType("decimal(18,2)");
        builder.Property(fgw => fgw.LastUpdated).HasColumnType("date");


        builder.HasOne(fgw => fgw.Income)
            .WithMany(i => i.FinishedGoodsWarehouses)
            .HasForeignKey(fgw => fgw.IncomeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}