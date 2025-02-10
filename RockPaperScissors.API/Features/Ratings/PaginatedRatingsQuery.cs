using api.Helpers;
using api.Helpers.CQRS;

namespace api.Features.Ratings;

public class PaginatedRatingsQuery: IQuery<PaginationWrapper<UserRatingDto>>
{
    public int Page { get; set; }
    public int Count { get; set; }
}