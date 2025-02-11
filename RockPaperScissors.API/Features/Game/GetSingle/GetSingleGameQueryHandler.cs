using api.DataAccess;
using api.Features.Game.Create;
using api.Helpers;
using api.Helpers.CQRS;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Game.GetSingle;

public class GetSingleGameQueryHandler: IQueryHandler<GetSingleGameQuery, Result<GameDto>>
{
    private readonly AppDbContext _dbContext;

    public GetSingleGameQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<GameDto>> Handle(GetSingleGameQuery request, CancellationToken cancellationToken)
    {
        var game = await _dbContext
            .Games
            .Include(g => g.Creator)
            .FirstOrDefaultAsync(g => g.Id == request.GameId);

        if (game == null)
        {
            return Result<GameDto>.Failure(StatusCodes.Status404NotFound, "Игры с таким id нет");
        }
        
        var gameDto = new GameDto()
        {
            GameId = game.Id,
            Date = game.CreatedAt,
            MaxRating = game.MaxRating,
            OwnerId = game.Creator.Id,
            OwnerName = game.Creator.Username,
            Status = game.GameStatus
        };
        
        return Result<GameDto>.Success(gameDto);
    }
}