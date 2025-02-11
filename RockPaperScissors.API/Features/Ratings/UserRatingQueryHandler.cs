using api.DataAccess;
using api.Helpers;
using api.Helpers.CQRS;
using api.Repositories;

namespace api.Features.Ratings;

public class UserRatingQueryHandler: IQueryHandler<UserRatingQuery, Result<bool>>
{
    private readonly IUserRatingsRepository _ratingsRepository;
    private readonly AppDbContext _dbContext;

    public UserRatingQueryHandler(IUserRatingsRepository ratingsRepository, AppDbContext dbContext)
    {
        _ratingsRepository = ratingsRepository;
        _dbContext = dbContext;
    }

    public async Task<Result<bool>> Handle(UserRatingQuery request, CancellationToken cancellationToken)
    {
        var userRating = await _ratingsRepository.GetSingle(request.UserId);
        if (userRating == null)
        {
            return Result<bool>.Failure(StatusCodes.Status404NotFound, "User rating not found");
        }

        var game = _dbContext.Games.FirstOrDefault(g => g.Id == request.GameId);
        if (game == null)
        {
            return Result<bool>.Failure(StatusCodes.Status404NotFound, "Game not found");
        }
        
        return Result<bool>.Success(game.MaxRating > userRating.Rating);
    }
}