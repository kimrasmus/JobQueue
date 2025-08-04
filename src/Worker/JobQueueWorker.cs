using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.DTOs;
using Shared.Entities;
using Shared.Interfaces;
using Shared.Services;

public class JobQueueWorker : BackgroundService
{
    private IJobQueueService _jobQueueService;
    private readonly IServiceProvider _provider;
    private readonly JobExecutorRegistry _registry;
    private readonly int _queueId;
    private readonly string _instanceName;

    public JobQueueWorker(
        IJobQueueService jobQueueService,
        IServiceProvider provider,
        JobExecutorRegistry registry,
        int queueId,
        string instanceName
    )
    {
        _jobQueueService = jobQueueService;
        _provider = provider;
        _registry = registry;
        _queueId = queueId;
        _instanceName = instanceName;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"STARTING: {_instanceName}");

        while (!stoppingToken.IsCancellationRequested)
        {
            using IServiceScope scope = _provider.CreateScope();
            int currentJobId = 0;
            try
            {
                // @TODO: Skal kunne forsinkes, hvis en anden worker konstaterer, at der ikke er jobs på køen
                JobQueueItem? job = await _jobQueueService.DequeueNextAsync(_queueId);
                if (job is null)
                {
                    // @TODO: Skal hænge sammen med de andre workers (de skal kunne kommunikere)
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }
                currentJobId = job.Id;

                Console.WriteLine($"{_instanceName} running job: {job.JobType}, id={job.Id}");

                Type? executorType = _registry.GetExecutorType(job.JobType);
                Type? inputType = _registry.GetInputDtoType(job.JobType);
                Type? contextType = _registry.GetContextDtoType(job.JobType);

                if (executorType == null || inputType == null || contextType == null)
                    throw new InvalidOperationException(
                        $"Job type '{job.JobType}' is not registered."
                    );

                var input =
                    JsonSerializer.Deserialize(job.InputPayload, inputType)
                    ?? throw new InvalidOperationException("Input deserialization failed.");
                var context =
                    JsonSerializer.Deserialize(job.WorkingContext, contextType)
                    ?? throw new InvalidOperationException("Context deserialization failed.");

                var executor =
                    scope.ServiceProvider.GetService(executorType)
                    ?? Activator.CreateInstance(executorType)
                    ?? throw new InvalidOperationException("Failed to instantiate executor.");

                MethodInfo method =
                    executorType.GetMethod(
                        "ExecuteAsync",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                    )
                    ?? throw new InvalidOperationException(
                        "Executor does not have ExecuteAsync method."
                    );

                object? taskObj = method.Invoke(executor, new[] { input, context, job, null });

                if (taskObj is Task<JobResultDto> task)
                {
                    JobResultDto result = await task;
                    if (result.Retry)
                    {
                        await Task.Delay(100); // Gør plads til andre ..
                        Console.WriteLine($"Retry job: {job.JobType}, id={job.Id}");
                        await _jobQueueService.Retry(job.Id, result);
                    }
                    else
                    {
                        await Task.Delay(100); // Gør plads til andre ..
                        await _jobQueueService.UpdateAndInsertAsync(job.Id, result);
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "ExecuteAsync did not return Task<JobResultDto>"
                    );
                }
            }
            catch (OperationCanceledException)
            {
                // Denne skal fanges, da den kastes hvis der afbrydes med "Task.Delay" er aktiv ..
                // Ingen handling, bare håndtering ..
            }
            catch (Exception ex)
            {
                // @TODO: Mangler implementering af HardRetry ..
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                JobResultDto result = new JobResultDto
                {
                    Status = JobResultDto.StatusFailed,
                    StatusMessage = ex.Message,
                    ExecutionResult = ex.StackTrace,
                };
                await _jobQueueService.UpdateAndInsertAsync(currentJobId, result);
                await Task.Delay(1000, stoppingToken);
            }
        }

        Console.WriteLine("");
        Console.WriteLine("****************************************************");
        Console.WriteLine($"STOPPING: {_instanceName}");
        Console.WriteLine("****************************************************");
        Console.WriteLine("");
    }
}
