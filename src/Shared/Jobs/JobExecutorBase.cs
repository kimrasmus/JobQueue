using System.Text.Json;
using Shared.DTOs;
using Shared.Entities;
using Shared.Interfaces;

namespace Shared.Jobs;

public abstract class JobExecutorBase<TInput, JobContextBase> : IJobExecutor<TInput, JobContextBase>
{
    private protected TInput _input { get; private set; } = default!;
    private protected JobContextBase _context { get; private set; } = default!;
    private protected JobQueueItem _self { get; private set; } = default!;
    private protected JobQueueItem? _parent { get; private set; }
    private Dictionary<string, Func<Task<JobResultDto>>> _stateHandlers = new();
    private string _initialState = String.Empty;

    public async Task<JobResultDto> ExecuteAsync(
        TInput input,
        JobContextBase context,
        JobQueueItem self,
        JobQueueItem? parent
    )
    {
        _input = input;
        _context = context;
        _self = self;
        _parent = parent;

        if (self.JobState is null || self.JobState == JobQueueItemDto.InitialJobState)
        {
            await InitializeAsync();
            return ChangeState(_initialState);
        }
        else if (self.JobState == JobQueueItemDto.FinishingJobState)
        {
            await FinishAsync();
            return Done();
        }
        else
        {
            return await RunAsync(self.JobState ?? JobQueueItemDto.InitialJobState);
        }
    }

    protected async Task<JobResultDto> RunAsync(string state)
    {
        if (_stateHandlers.TryGetValue(state, out Func<Task<JobResultDto>>? handler))
        {
            return await handler();
        }
        else
        {
            return Fail($"Unknown state: {state}");
        }
    }

    protected abstract Task InitializeAsync();
    protected abstract Task FinishAsync();

    protected void AddStateHandler(
        string state,
        Func<Task<JobResultDto>> f,
        bool startingPoint = false
    )
    {
        _stateHandlers.Add(state, f);
        if (startingPoint)
        {
            _initialState = state;
        }
    }

    protected JobResultDto ChangeState(string newState)
    {
        List<JobQueueItemDto> items = new List<JobQueueItemDto>
        {
            new JobQueueItemDto
            {
                ParentJobId = _self.Id,
                RunId = _self.RunId,
                QueueId = _self.QueueId,
                JobType = _self.JobType,
                JobState = newState,
                InputPayload = _self.InputPayload,
                WorkingContext = JsonSerializer.Serialize(_context),
                ScheduledAt = DateTime.UtcNow,
            },
        };

        return new JobResultDto()
        {
            Status = JobResultDto.StatusDone,
            WorkingContext = JsonSerializer.Serialize(_context),
            items = items,
        };
    }

    protected JobResultDto Finish()
    {
        List<JobQueueItemDto> items = new List<JobQueueItemDto>
        {
            new JobQueueItemDto
            {
                ParentJobId = _self.Id,
                RunId = _self.RunId,
                QueueId = _self.QueueId,
                JobType = _self.JobType,
                JobState = JobQueueItemDto.FinishingJobState,
                InputPayload = _self.InputPayload,
                WorkingContext = JsonSerializer.Serialize(_context),
                ScheduledAt = DateTime.UtcNow,
            },
        };

        return new JobResultDto()
        {
            Status = JobResultDto.StatusDone,
            WorkingContext = JsonSerializer.Serialize(_context),
            items = items,
        };
    }

    private JobResultDto Done()
    {
        return new JobResultDto()
        {
            Status = JobResultDto.StatusDone,
            WorkingContext = JsonSerializer.Serialize(_context),
        };
    }

    protected JobResultDto Fail(string message)
    {
        return new JobResultDto() { Status = JobResultDto.StatusFailed, StatusMessage = message };
    }

    protected JobResultDto Retry(Exception ex, int delay)
    {
        return new JobResultDto()
        {
            Retry = true,
            RetryDelay = delay,
            StatusMessage = ex.Message,
            ExecutionResult = ex.StackTrace,
        };
    }
}
