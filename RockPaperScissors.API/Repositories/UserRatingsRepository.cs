using api.Domain;
using api.Helpers;
using MongoDB.Bson;
using MongoDB.Driver;

namespace api.Repositories;

public class UserRatingsRepository: IUserRatingsRepository
{
    private readonly IMongoCollection<UserRating> _ratings;

    public UserRatingsRepository(IMongoDatabase mongoDatabase)
    {
        _ratings = mongoDatabase.GetCollection<UserRating>(Constants.MongoDbRatingCollection);
    }
    
    public async Task<PaginationWrapper<UserRating>> GetAllByPagination(int page, int count)
    {
        if (page <= 0) page = 1; 
        var sort = Builders<UserRating>.Sort.Descending(entity => entity.Rating);
        var data = await _ratings
            .Find(new BsonDocument())
            .Sort(sort)
            .Skip((page - 1) * count)
            .Limit(count)
            .ToListAsync();
        var countTotal = await _ratings.CountDocumentsAsync(new BsonDocument());

        return new PaginationWrapper<UserRating>(data, page, count, countTotal);
    }

    public async Task InsertOneAsync(Guid userId, int rating)
    {
        await _ratings.InsertOneAsync(new UserRating
        {
            UserId = userId,
            Rating = 0
        });
    }
}