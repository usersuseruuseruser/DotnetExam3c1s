using api.DataAccess;
using api.Helpers;
using api.Helpers.CQRS;
using api.Repositories;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace api.Features.Ratings;

public class PaginatedRatingsQueryHandler: IQueryHandler<PaginatedRatingsQuery, PaginationWrapper<UserRatingDto>>
{
    private readonly AppDbContext _dbContext;
    private readonly IUserRatingsRepository _ratingsRepository;

    public PaginatedRatingsQueryHandler(IMongoDatabase mongoDatabase, AppDbContext dbContext, IUserRatingsRepository ratingsRepository)
    {
        _dbContext = dbContext;
        _ratingsRepository = ratingsRepository;
    }

    public async Task<PaginationWrapper<UserRatingDto>> Handle(PaginatedRatingsQuery request, CancellationToken cancellationToken)
    {
        var data = await _ratingsRepository.GetAllByPagination(request.Page, request.Count);
        
        var resData = new List<UserRatingDto>();
        var userIds = data.Data.Select(ur => ur.UserId).ToList(); 
        var usernames = await _dbContext
            .Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u =>
                new
                {
                    u.Id,
                    u.Username
                })
            .ToListAsync();
            

        foreach (var userRating in data.Data)
        {
            resData.Add(new UserRatingDto()
            {
                Username = usernames.First(u => u.Id == userRating.UserId).Username,
                Rating = userRating.Rating
            });
        }
        
        return new PaginationWrapper<UserRatingDto>(resData, request.Page, request.Count, data.TotalCount);
    }
}