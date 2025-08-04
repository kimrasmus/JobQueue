using Shared.Interfaces;

namespace Shared.Services;

public class JobExecutorRegistry
{
    private readonly Dictionary<string, Type> _executorRegistry = new();
    private readonly Dictionary<string, Type> _inputDtoRegistry = new();
    private readonly Dictionary<string, Type> _contextDtoRegistry = new();

    public void Register(Type executor, Type input, Type context, string jobType)
    {
        _executorRegistry[jobType] = executor;
        _inputDtoRegistry[jobType] = input;
        _contextDtoRegistry[jobType] = context;
    }

    public Type? GetExecutorType(string jobType) => _executorRegistry.GetValueOrDefault(jobType);

    public Type? GetInputDtoType(string jobType) => _inputDtoRegistry.GetValueOrDefault(jobType);

    public Type? GetContextDtoType(string jobType) =>
        _contextDtoRegistry.GetValueOrDefault(jobType);
}
