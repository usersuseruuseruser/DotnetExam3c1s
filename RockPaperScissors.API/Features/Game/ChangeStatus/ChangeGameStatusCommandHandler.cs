using api.DataAccess;
using api.Domain;
using api.Helpers;
using api.Helpers.CQRS;

namespace api.Features.Game.ChangeStatus;

public class ChangeGameStatusCommandHandler: ICommandHandler<ChangeGameStatusCommand, Result<string>>
{
    private readonly AppDbContext _dbContext;

    public ChangeGameStatusCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<string>> Handle(ChangeGameStatusCommand request, CancellationToken cancellationToken)
    {
        var game = _dbContext.Games.FirstOrDefault(g => g.Id == request.GameId);
        
        if (game == null)
        {
            return Result<string>.Failure(StatusCodes.Status404NotFound, "Game not found");
        }
        
        game.GameStatus = request.Status;

        if (request.Status == GameStatus.Created)
        {
            game.Messages.Add(new ChatMessage()
            {
                Text = "Игра была создана",
                CreatedAt = DateTime.UtcNow
            });
        }
        
        if (request.Status == GameStatus.Started)
        {
            game.Messages.Add(new ChatMessage()
            {
                Text = "Игра началась",
                CreatedAt = DateTime.UtcNow
            });
        }
        
        if (request.Status == GameStatus.Finished)
        {
            game.Messages.Add(new ChatMessage()
            {
                Text = "Игра завершена",
                CreatedAt = DateTime.UtcNow
            });
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Result<string>.Success("Game status changed successfully");
    }
}