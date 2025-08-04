using Shared.DTOs;

namespace Shared.Interfaces;

public interface IJobContext
{
    List<LoggedWorkDto> LoggedWork { get; set; }
    void LogWork(string work);
}
