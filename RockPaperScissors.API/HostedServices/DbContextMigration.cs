using api.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace api.HostedServices;

public class DbContextMigration : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DbContextMigration(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var pendingMigrations = await dbContext.Database
            .GetPendingMigrationsAsync(cancellationToken: stoppingToken);

        if (pendingMigrations.Any())
            await dbContext.Database.MigrateAsync(cancellationToken: stoppingToken);
        
    }
}