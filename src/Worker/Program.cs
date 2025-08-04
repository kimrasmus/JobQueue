using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Entities;
using Shared.Extensions;
using Shared.Infrastructure.Persistence;
using Shared.Interfaces;
using Shared.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(
        (context, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        }
    )
    .ConfigureServices(
        (context, services) =>
        {
            var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContextFactory<QueueDbContext>(options =>
                options.UseSqlServer(connectionString)
            );

            services.AddSharedServices();

            var queueConfigs =
                context.Configuration.GetSection("QueueWorkers").Get<List<QueueWorkerConfig>>()
                ?? new();

            Dictionary<string, int> queueIds = new();
            Dictionary<string, JobQueueItem?> jobs = new();
            foreach (var config in queueConfigs)
            {
                for (int i = 0; i < config.Concurrency; i++)
                {
                    int instanceNo = i + 1;
                    string instanceName = $"Q{config.QueueId}I{instanceNo}";
                    services.AddSingleton<IHostedService>(provider =>
                    {
                        JobExecutorRegistry registry =
                            provider.GetRequiredService<JobExecutorRegistry>();
                        IJobQueueService jobQueueService =
                            provider.GetRequiredService<IJobQueueService>();
                        JobQueueWorker worker = new JobQueueWorker(
                            jobQueueService,
                            provider,
                            registry,
                            config.QueueId,
                            instanceName
                        );
                        return new NamedHostedService(worker);
                    });
                }
            }
        }
    )
    .Build();

await host.RunAsync();
