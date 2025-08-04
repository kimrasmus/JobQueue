namespace Shared.Entities;

public class JobQueueItem
{
    public int Id { get; set; }
    public int? ParentJobId { get; set; }
    public string RunId { get; set; } = String.Empty;
    public int QueueId { get; set; } = 1;
    public string JobType { get; set; } = String.Empty;
    public string JobState { get; set; } = String.Empty;
    public string InputPayload { get; set; } = String.Empty;
    public string WorkingContext { get; set; } = "{}";
    public string? ExecutionResult { get; set; }
    public DateTime ScheduledAt { get; set; } = DateTime.UtcNow;
    public int RetryCount { get; set; } = 0;
    public int HardRetryCount { get; set; } = 0;
    public string JobStatus { get; set; } = "waiting"; // waiting, on_hold, working, done, failed
    public string? StatusDetail { get; set; }
    public string CreatedByUser { get; set; } = String.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
}
