using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.DTOs;
using Shared.Entities;
using Shared.Infrastructure.Persistence;
using Shared.Interfaces;

namespace Shared.Services;

public class JobQueueService : IJobQueueService
{
    private readonly IDbContextFactory<QueueDbContext> _dbFactory;
    private readonly JobExecutorRegistry _registry;
    private readonly IUserService _userService;

    public JobQueueService(
        IDbContextFactory<QueueDbContext> dbFactory,
        JobExecutorRegistry registry,
        IUserService userService
    )
    {
        _dbFactory = dbFactory;
        _registry = registry;
        _userService = userService;
    }

    public async Task<JobQueueItem?> DequeueNextAsync(int queueId)
    {
        using QueueDbContext db = _dbFactory.CreateDbContext();
        //var stopwatch = Stopwatch.StartNew();
        await using var tx = await db.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.ReadCommitted
        );
        var job = await db
            .JobQueueItems.FromSqlInterpolated(
                $@"
                    SELECT TOP(1) *
                    FROM JobQueueItems WITH (ROWLOCK, READPAST, UPDLOCK)
                    WHERE QueueId = {queueId}
                    AND JobStatus = {JobResultDto.StatusWaiting}
                    AND ScheduledAt <= SYSUTCDATETIME()
                    ORDER BY ParentJobId DESC
                "
            )
            .FirstOrDefaultAsync();

        if (job is not null)
        {
            job.JobStatus = "working";
            job.StartedAt = DateTime.UtcNow;
            job.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }

        await tx.CommitAsync();

        //stopwatch.Stop();
        //Console.WriteLine($"DequeueNextAsync[{queueId}] tog {stopwatch.ElapsedMilliseconds} ms");

        return job;
    }

    public async Task<List<JobQueueItem>> CreateAsync(List<JobQueueItemDto> dtoList)
    {
        using QueueDbContext db = _dbFactory.CreateDbContext();
        await using var tx = await db.Database.BeginTransactionAsync();

        ValidateInputPayload(dtoList);
        List<JobQueueItem> entityList = await InsertJobsAsync(db, dtoList);

        await tx.CommitAsync();
        return entityList;
    }

    public async Task Retry(int id, JobResultDto result)
    {
        using QueueDbContext db = _dbFactory.CreateDbContext();
        var entity = await db.JobQueueItems.FindAsync(id);
        if (entity is null)
            throw new InvalidOperationException($"JobQueueItem med ID {id} blev ikke fundet.");

        entity.ScheduledAt = entity.ScheduledAt.AddSeconds(result.RetryDelay);
        entity.RetryCount++;
        entity.JobStatus = "waiting";
        entity.StartedAt = null;
        entity.StatusDetail = result.StatusMessage;
        entity.ExecutionResult = result.ExecutionResult;

        await db.SaveChangesAsync();
    }

    public async Task<List<JobQueueItem>> UpdateAndInsertAsync(int id, JobResultDto result)
    {
        using QueueDbContext db = _dbFactory.CreateDbContext();
        await using var tx = await db.Database.BeginTransactionAsync();

        await UpdateJobAsync(db, id, result);

        var entityList = new List<JobQueueItem>();
        if (result.items is not null)
        {
            ValidateInputPayload(result.items);
            entityList = await InsertJobsAsync(db, result.items);
        }

        await tx.CommitAsync();
        return entityList;
    }

    private async Task UpdateJobAsync(QueueDbContext db, int id, JobResultDto result)
    {
        var job = await db.JobQueueItems.FindAsync(id);
        if (job is null)
            throw new InvalidOperationException($"Job med ID {id} blev ikke fundet.");

        job.JobStatus = result.Status;
        job.StatusDetail = result.StatusMessage;
        job.ExecutionResult = result.ExecutionResult;
        job.WorkingContext = result.WorkingContext;

        await db.SaveChangesAsync();
    }

    private async Task<List<JobQueueItem>> InsertJobsAsync(
        QueueDbContext db,
        List<JobQueueItemDto> dtoList
    )
    {
        var entities = new List<JobQueueItem>();

        foreach (var dto in dtoList)
        {
            var entity = dto.toEntity();
            entity.CreatedByUser = _userService.GetLoggedInUser().Username;
            entities.Add(entity);
        }

        db.JobQueueItems.AddRange(entities);
        await db.SaveChangesAsync();

        return entities.ToList();
    }

    private void ValidateInputPayload(List<JobQueueItemDto> dtoList)
    {
        foreach (var dto in dtoList)
        {
            if (dto.JobType is null)
                throw new ArgumentException("Job type is missing");

            var inputType =
                _registry.GetInputDtoType(dto.JobType)
                ?? throw new InvalidOperationException($"Unknown job type: {dto.JobType}");

            var input =
                JsonSerializer.Deserialize(dto.InputPayload ?? "{}", inputType) as IJobInput;

            if (input is null || !input.IsValid())
                throw new ArgumentException($"Invalid input payload for job type: {dto.JobType}");
        }
    }
}
