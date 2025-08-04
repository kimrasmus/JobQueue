using System.Collections.Concurrent;
using Shared.DTOs;
using Shared.Entities;
using Shared.Interfaces;

namespace Shared.Services;

public class JobQueueServiceSequencer : IJobQueueService
{
    private readonly IJobQueueService _inner;

    private readonly SemaphoreSlim _dequeueLock = new(4, 4);
    private readonly SemaphoreSlim _generalConcurrency = new(8, 8);

    public JobQueueServiceSequencer(IJobQueueService inner)
    {
        _inner = inner;
    }

    private async Task<T> ExecuteWithSemaphore<T>(SemaphoreSlim semaphore, Func<Task<T>> action)
    {
        await semaphore.WaitAsync();
        try
        {
            return await action();
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task ExecuteWithSemaphore(SemaphoreSlim semaphore, Func<Task> action)
    {
        await semaphore.WaitAsync();
        try
        {
            await action();
        }
        finally
        {
            semaphore.Release();
        }
    }

    public Task<JobQueueItem?> DequeueNextAsync(int queueId) =>
        ExecuteWithSemaphore(_dequeueLock, () => _inner.DequeueNextAsync(queueId));

    public Task<List<JobQueueItem>> CreateAsync(List<JobQueueItemDto> dtoList) =>
        ExecuteWithSemaphore(_generalConcurrency, () => _inner.CreateAsync(dtoList));

    public Task<List<JobQueueItem>> UpdateAndInsertAsync(int id, JobResultDto result) =>
        ExecuteWithSemaphore(_generalConcurrency, () => _inner.UpdateAndInsertAsync(id, result));

    public Task Retry(int id, JobResultDto result) =>
        ExecuteWithSemaphore(_generalConcurrency, () => _inner.Retry(id, result));
}
