using EmployedNow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployedNow.Infrastructure.Data.Configurations;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    /// <summary>
    /// Configures application schema and duplicate-application unique index.
    /// </summary>
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.ToTable("JobApplications");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AppliedAt)
            .IsRequired();

        builder.HasIndex(x => new { x.JobPostingId, x.UserId })
            .IsUnique();

        builder.HasOne(x => x.JobPosting)
            .WithMany(x => x.Applications)
            .HasForeignKey(x => x.JobPostingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.JobApplications)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
