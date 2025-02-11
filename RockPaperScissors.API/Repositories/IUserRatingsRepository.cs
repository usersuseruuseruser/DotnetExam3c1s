using api.Domain;
using api.Helpers;

namespace api.Repositories;

public interface IUserRatingsRepository
{
    public Task<PaginationWrapper<UserRating>> GetAllByPagination(int page, int count);
    public Task<UserRating?> GetSingle(Guid userId);
    public Task InsertOneAsync(Guid userId, int rating);
    public Task UpdateOneAsync(Guid userId, int rating);
}