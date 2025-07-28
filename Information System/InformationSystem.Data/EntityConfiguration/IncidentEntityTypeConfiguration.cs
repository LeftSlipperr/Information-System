using InformationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfiguration;

public class IncidentEntityTypeConfiguration: IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.ToTable("Incidents");
        builder.HasKey(i => i.IncidentId);

        builder.Property(i => i.IncidentName).IsRequired();
        builder.Property(i => i.IncidentDate).IsRequired().HasColumnType("date");
        builder.Property(i => i.IncidentDescription);
    }
}
