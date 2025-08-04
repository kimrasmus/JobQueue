using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Shared.Infrastructure.Persistence;

namespace Shared.Infrastructure.Persistence;

public class QueueDbContextFactory : IDesignTimeDbContextFactory<QueueDbContext>
{
    public QueueDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<QueueDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new QueueDbContext(optionsBuilder.Options);
    }
}
