using api.DataAccess;
using api.Domain;
using api.Helpers;
using api.Helpers.CQRS;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Game.Leave;

public class LeaveGameCommandHandler: ICommandHandler<LeaveGameCommand, Result<bool>>
{
    private readonly AppDbContext _dbContext;

    public LeaveGameCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<bool>> Handle(LeaveGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _dbContext
            .Games
            .Include(g => g.FirstPlayer)
            .Include(g => g.SecondPlayer)
            .FirstOrDefaultAsync(g => g.Id == request.GameId, cancellationToken: cancellationToken);
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken: cancellationToken);
        
        if (game == null)
        {
            return Result<bool>.Failure(StatusCodes.Status404NotFound, "Game not found");
        }
        
        if (game.FirstPlayerId == request.UserId)
        {
            game.FirstPlayerId = game.SecondPlayerId;
            game.SecondPlayerId = null;
        }
        else if (game.SecondPlayerId == request.UserId)
            game.SecondPlayerId = null;
        
        game.Messages.Add(new ChatMessage()
        {
            Text = $"Пользователь {user!.Username} покинул игру",
            CreatedAt = DateTime.UtcNow
        });
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Result<bool>.Success(true);
    }
}