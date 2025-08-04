using Microsoft.Extensions.Hosting;

public class NamedHostedService : IHostedService
{
    private readonly BackgroundService _service;

    public NamedHostedService(BackgroundService service)
    {
        _service = service;
    }

    public Task StartAsync(CancellationToken cancellationToken) =>
        _service.StartAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) =>
        _service.StopAsync(cancellationToken);
}
