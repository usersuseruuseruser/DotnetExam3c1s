using api.Helpers;
using api.Helpers.CQRS;

namespace api.Features.Game.Join;

public class JoinGameCommand: ICommand<Result<string>>
{
    public Guid GameId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; }
}