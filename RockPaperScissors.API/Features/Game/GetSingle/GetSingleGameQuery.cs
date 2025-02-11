using api.Features.Game.Create;
using api.Helpers;
using api.Helpers.CQRS;

namespace api.Features.Game.GetSingle;

public class GetSingleGameQuery: IQuery<Result<GameDto>>
{
    public Guid GameId { get; set; }
}