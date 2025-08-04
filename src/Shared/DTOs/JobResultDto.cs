namespace Shared.DTOs;

public class JobResultDto
{
    public const string StatusWaiting = "waiting";
    public const string StatusOnHold = "on_hold";
    public const string StatusDone = "done";
    public const string StatusFailed = "failed";

    public string Status { get; set; } = StatusWaiting;
    public bool Retry { get; set; } = false;
    public int RetryDelay { get; set; } = 0;
    public string? StatusMessage { get; set; }
    public string? ExecutionResult { get; set; }
    public string WorkingContext { get; set; } = "{}";
    public List<JobQueueItemDto>? items { get; set; }
}
