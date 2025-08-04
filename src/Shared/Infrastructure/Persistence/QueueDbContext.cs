using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace Shared.Infrastructure.Persistence;

public class QueueDbContext : DbContext
{
    public DbSet<JobQueueItem> JobQueueItems => Set<JobQueueItem>();

    public QueueDbContext(DbContextOptions<QueueDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new Configurations.JobQueueItemConfiguration());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<JobQueueItem>())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
