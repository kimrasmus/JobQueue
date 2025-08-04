using System.Text.Json;
using Shared.DTOs;
using Shared.Entities;
using Shared.Interfaces;

namespace Shared.Jobs;

public abstract class JobContextBase : IJobContext
{
    public List<LoggedWorkDto> LoggedWork { get; set; } = new();

    public void LogWork(string work)
    {
        LoggedWork.Add(new LoggedWorkDto { Created = DateTime.UtcNow, Work = work });
    }
}
