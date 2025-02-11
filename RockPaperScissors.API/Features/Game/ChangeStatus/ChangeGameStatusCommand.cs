using api.Domain;
using api.Helpers;
using api.Helpers.CQRS;

namespace api.Features.Game.ChangeStatus;

public class ChangeGameStatusCommand: ICommand<Result<string>>
{
    public Guid GameId { get; set; }
    public GameStatus Status { get; set; }
}