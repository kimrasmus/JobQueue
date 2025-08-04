using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces;
using Shared.Services;

namespace Shared.Extensions;

public static class ServiceRegistration
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        services.AddScoped<IJobQueueService, JobQueueService>();
        services.Decorate<IJobQueueService, JobQueueServiceSequencer>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUnstableServices, UnstableServices>();

        services.AddJobExecutors();

        return services;
    }
}
