using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Entities;

namespace Shared.Infrastructure.Persistence.Configurations;

public class JobQueueItemConfiguration : IEntityTypeConfiguration<JobQueueItem>
{
    public void Configure(EntityTypeBuilder<JobQueueItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RunId).IsRequired().HasMaxLength(64);
        builder.Property(x => x.JobType).IsRequired().HasMaxLength(128);
        builder.Property(x => x.JobState).IsRequired().HasMaxLength(128);
        builder.Property(x => x.InputPayload).IsRequired();
        builder.Property(x => x.JobStatus).IsRequired().HasMaxLength(16);
        builder.Property(x => x.StatusDetail).HasMaxLength(512);
        builder.Property(x => x.CreatedByUser).IsRequired().HasMaxLength(16);
        builder.Property(x => x.ScheduledAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.RetryCount).IsRequired();

        builder.HasIndex(x => x.ScheduledAt);
        builder.HasIndex(x => x.JobStatus);
        builder.HasIndex(x => x.RunId);
        builder.HasIndex(x => x.JobType);
        builder.HasIndex(x => x.StatusDetail);
        builder.HasIndex(x => x.RetryCount);
        builder.HasIndex(x => x.HardRetryCount);
        builder.HasIndex(x => x.UpdatedAt);
        builder.HasIndex(x => x.StartedAt);
        builder.HasIndex(x => x.ParentJobId);
        builder.HasIndex(x => new
        {
            x.JobStatus,
            x.ScheduledAt,
            x.JobType,
            x.Id,
        });
        builder.HasIndex(x => new
        {
            x.RunId,
            x.JobStatus,
            x.ScheduledAt,
        });
        builder.HasIndex(x => new
        {
            x.JobStatus,
            x.ScheduledAt,
            x.JobType,
        });
    }
}
