using api.DataAccess;
using api.Domain;
using api.Helpers;
using api.Helpers.CQRS;

namespace api.Features.Game.Create;

public class CreateGameCommandHandler: ICommandHandler<CreateGameCommand, Result<GameDto>>
{
    private readonly AppDbContext _dbContext;

    public CreateGameCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<GameDto>> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        var game = new Domain.Game()
        {
            Id = Guid.NewGuid(),
            CreatorId = request.UserId,
            CreatedAt = DateTime.UtcNow,
            GameStatus = GameStatus.Created,
            MaxRating = request.MaxRating
        };

        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync(cancellationToken);
        var owner = _dbContext.Users.FirstOrDefault(u => u.Id == request.UserId);

        if (owner == null)
        {
            return Result<GameDto>.Failure(StatusCodes.Status400BadRequest, "Юзера с вашим id нет");
        }
        
        var gameDto = new GameDto()
        {
            GameId = game.Id,
            Date = game.CreatedAt,
            MaxRating = request.MaxRating,
            OwnerId = request.UserId,
            OwnerName = owner.Username,
            Status = GameStatus.Created
        };

        return Result<GameDto>.Success(gameDto);
    }
}