using api.Domain;
using api.Helpers;

namespace api.Repositories;

public interface IUserRatingsRepository
{
    public Task<PaginationWrapper<UserRating>> GetAllByPagination(int page, int count);
    public Task InsertOneAsync(Guid userId, int rating);
}