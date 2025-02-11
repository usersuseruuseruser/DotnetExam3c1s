using api.Helpers;
using api.Helpers.CQRS;

namespace api.Features.Game.Leave;

public class LeaveGameCommand: ICommand<Result<bool>>
{
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
}