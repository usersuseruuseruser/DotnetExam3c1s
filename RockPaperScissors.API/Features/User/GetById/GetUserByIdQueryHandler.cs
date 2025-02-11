using api.DataAccess;
using api.Helpers;
using api.Helpers.CQRS;
using Microsoft.EntityFrameworkCore;

namespace api.Features.User.GetById;

public class GetUserByIdQueryHandler: IQueryHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly AppDbContext _dbContext;

    public GetUserByIdQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .Where(u => u.Id == request.UserId)
            .Select(u => new UserDto(u.Id, u.Username))
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            return Result<UserDto>.Failure(StatusCodes.Status404NotFound, "Пользователь не найден");
        }

        return Result<UserDto>.Success(user);
    }
}