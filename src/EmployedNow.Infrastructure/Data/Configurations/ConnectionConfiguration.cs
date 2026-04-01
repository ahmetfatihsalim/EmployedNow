using EmployedNow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployedNow.Infrastructure.Data.Configurations;

public class ConnectionConfiguration : IEntityTypeConfiguration<Connection>
{
    /// <summary>
    /// Configures directional connection request relationships between users.
    /// </summary>
    public void Configure(EntityTypeBuilder<Connection> builder)
    {
        builder.ToTable("Connections");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => new { x.RequesterId, x.TargetId, x.Status });

        builder.HasOne(x => x.Requester)
            .WithMany(x => x.SentConnections)
            .HasForeignKey(x => x.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Target)
            .WithMany(x => x.ReceivedConnections)
            .HasForeignKey(x => x.TargetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
