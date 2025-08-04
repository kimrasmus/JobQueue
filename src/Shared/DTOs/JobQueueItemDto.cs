using Shared.Entities;

namespace Shared.DTOs;

public class JobQueueItemDto
{
    public const string InitialJobState = "[init]";
    public const string FinishingJobState = "[finish]";

    public int? ParentJobId { get; set; }
    public string RunId { get; set; } = Guid.NewGuid().ToString();
    public int QueueId { get; set; } = 1;
    public string JobType { get; set; } = String.Empty;
    public string JobState { get; set; } = InitialJobState;
    public string InputPayload { get; set; } = "{}";
    public string WorkingContext { get; set; } = "{}";
    public DateTime ScheduledAt { get; set; } = DateTime.UtcNow;

    public JobQueueItem toEntity()
    {
        return new JobQueueItem()
        {
            ParentJobId = ParentJobId,
            RunId = RunId,
            QueueId = QueueId,
            JobType = JobType,
            JobState = JobState,
            InputPayload = InputPayload,
            WorkingContext = WorkingContext,
            ScheduledAt = ScheduledAt,
        };
    }
}
