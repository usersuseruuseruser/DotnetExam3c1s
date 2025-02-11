using System.Text.Json.Serialization;
using api.Helpers;
using api.Helpers.CQRS;

namespace api.Features.Game.GetMessages;

public class GameMessagesQuery: IQuery<Result<GameMessagesDto>>
{
    public Guid GameId { get; set; }
}