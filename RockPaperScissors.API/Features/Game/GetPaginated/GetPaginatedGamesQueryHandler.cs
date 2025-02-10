using api.DataAccess;
using api.Domain;
using api.Features.Game.Create;
using api.Helpers;
using api.Helpers.CQRS;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Game.GetPaginated;

public class GetPaginatedGamesQueryHandler: IQueryHandler<GetPaginatedGamesQuery, Result<PaginationWrapper<GameDto>>>
{
    private readonly AppDbContext _dbContext;

    public GetPaginatedGamesQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PaginationWrapper<GameDto>>> Handle(GetPaginatedGamesQuery request, CancellationToken cancellationToken)
    {
        var games = await _dbContext.Games
            .Include(g => g.Creator)
            .Select(g => new
            {
                Game = g,
                SortKey = (g.GameStatus == GameStatus.Created ? 1 : 0) +
                          (g.GameStatus == GameStatus.Started ? 2 : 0) + 
                          (g.GameStatus == GameStatus.Finished ? 3 : 0)
                    
            })
            .OrderBy(g => g.SortKey)
            .ThenByDescending(g => g.Game.CreatedAt)
            .Skip(request.Page * request.Count)
            .Take(request.Count)
            .Select(g => new GameDto()
            {
                GameId = g.Game.Id,
                OwnerId = g.Game.Creator.Id,
                OwnerName = g.Game.Creator.Username,
                Date = g.Game.CreatedAt,
                MaxRating = g.Game.MaxRating,
                Status = g.Game.GameStatus
            })
            .ToListAsync(cancellationToken: cancellationToken);

        var total = await _dbContext.Games.CountAsync(cancellationToken: cancellationToken);
        
        return Result<PaginationWrapper<GameDto>>.Success(new PaginationWrapper<GameDto>(games, request.Page,request.Count, total));
    }
}