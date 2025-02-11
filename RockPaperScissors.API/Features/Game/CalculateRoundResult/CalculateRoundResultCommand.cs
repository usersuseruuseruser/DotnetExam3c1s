using api.Contracts;
using api.Helpers;
using api.Helpers.CQRS;
using api.Hubs;

namespace api.Features.Game.CalculateRoundResult;

public class CalculateRoundResultCommand: ICommand<Result<RoundFinishDto>>
{
    public UserMove UserMove1 { get; set; }
    public UserMove UserMove2 { get; set; }
    public Guid GameId { get; set; }
}