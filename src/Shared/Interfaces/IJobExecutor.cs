using Shared.DTOs;
using Shared.Entities;

namespace Shared.Interfaces;

public interface IJobExecutor<TInput, TContext>
{
    Task<JobResultDto> ExecuteAsync(
        TInput input,
        TContext context,
        JobQueueItem self,
        JobQueueItem? parent
    );
}
