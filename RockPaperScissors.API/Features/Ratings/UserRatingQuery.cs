using api.Helpers;
using api.Helpers.CQRS;

namespace api.Features.Ratings;

public class UserRatingQuery: IQuery<Result<bool>>
{
    public Guid GameId { get; set; }
    public Guid UserId { get; set; }
}