using api.DataAccess;
using api.Domain;
using Microsoft.EntityFrameworkCore;

namespace api.HostedServices;

public class OldGamesRemover: BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(30));
    
    public OldGamesRemover(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        while (await _timer.WaitForNextTickAsync(stoppingToken)
               && !stoppingToken.IsCancellationRequested)
        {
            var count = await dbContext.Games
                .Where(g => g.CreatedAt <= DateTime.Now - TimeSpan.FromHours(24) &&
                            g.FirstPlayer == null &&
                            g.SecondPlayer == null)
                .ExecuteUpdateAsync(setters => setters.SetProperty(g => g.GameStatus, GameStatus.Finished), cancellationToken: stoppingToken);
        }
    }
}