using Shared.DTOs;
using Shared.Entities;

namespace Shared.Interfaces;

public interface IJobQueueService
{
    Task<JobQueueItem?> DequeueNextAsync(int queueId);
    Task<List<JobQueueItem>> CreateAsync(List<JobQueueItemDto> dtoList);
    Task<List<JobQueueItem>> UpdateAndInsertAsync(int id, JobResultDto result);
    Task Retry(int id, JobResultDto result);
}
