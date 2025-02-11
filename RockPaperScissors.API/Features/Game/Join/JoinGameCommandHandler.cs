using api.DataAccess;
using api.Domain;
using api.Helpers;
using api.Helpers.CQRS;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Game.Join;

public class JoinGameCommandHandler: ICommandHandler<JoinGameCommand, Result<string>>
{
    private readonly AppDbContext _dbContext;

    public JoinGameCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<string>> Handle(JoinGameCommand request, CancellationToken cancellationToken)
    {
        var game = _dbContext
            .Games
            .Include(g => g.FirstPlayer)
            .Include(g => g.SecondPlayer)
            .FirstOrDefault(g => g.Id == request.GameId);

        if (game == null)
        {
            return Result<string>.Failure(StatusCodes.Status404NotFound, "Game not found");
        }

        if (game.FirstPlayer != null && game.SecondPlayer != null)
        {
            return Result<string>.Failure(StatusCodes.Status400BadRequest, "Game is already full");
        }
        
        if (game.FirstPlayer == null)
            game.FirstPlayerId = request.UserId;
        else if (game.SecondPlayer == null)
            game.SecondPlayerId = request.UserId;
        game.Messages.Add(new ChatMessage()
        {
            Text = $"Пользователь {request.Username} присоединился к игре",
            CreatedAt = DateTime.UtcNow
        });
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Result<string>.Success("Successfully joined the game");
    }
}