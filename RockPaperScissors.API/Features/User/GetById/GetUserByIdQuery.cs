using api.Helpers;
using api.Helpers.CQRS;

namespace api.Features.User.GetById;

public class GetUserByIdQuery: IQuery<Result<UserDto>>
{
    public Guid UserId { get; set; }
}
public record UserDto(Guid Id, string Username);