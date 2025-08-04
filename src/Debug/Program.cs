using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Extensions;
using Shared.Infrastructure.Persistence;
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
            services.AddScoped<Debug>();
        }
    )
    .Build();

using var scope = host.Services.CreateScope();
var debug = scope.ServiceProvider.GetRequiredService<Debug>();
await debug.RunAsync();
