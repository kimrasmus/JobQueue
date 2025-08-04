using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces;
using Shared.Jobs;
using Shared.Services;

namespace Shared.Extensions;

public static class JobExecutorServiceExtensions
{
    public static IServiceCollection AddJobExecutors(this IServiceCollection services)
    {
        var registry = new JobExecutorRegistry();

        foreach (string name in JobConfig.JobNames)
        {
            var ns = $"Shared.Jobs.{name}";

            var executorType = Type.GetType($"{ns}.{name}Executor, Shared");
            var inputType = Type.GetType($"{ns}.{name}InputDto, Shared");
            var contextType = Type.GetType($"{ns}.{name}ContextDto, Shared");

            if (executorType is null || inputType is null || contextType is null)
                throw new InvalidOperationException($"Typer for job '{name}' kunne ikke findes.");

            services.AddTransient(executorType);
            registry.Register(executorType, inputType, contextType, name);
        }

        services.AddSingleton(registry);
        return services;
    }
}
