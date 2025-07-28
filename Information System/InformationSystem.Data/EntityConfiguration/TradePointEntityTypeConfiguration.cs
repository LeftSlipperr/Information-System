using InformationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfiguration;

public class TradePointEntityTypeConfiguration: IEntityTypeConfiguration<TradePoint>
{
    public void Configure(EntityTypeBuilder<TradePoint> builder)
    {
        builder.ToTable("TradePoints");
        builder.HasKey(tp => tp.TradePointId);

        builder.Property(tp => tp.Name).IsRequired();
        builder.Property(tp => tp.Address);

    }
}
