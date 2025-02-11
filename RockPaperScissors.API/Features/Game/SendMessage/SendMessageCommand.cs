using api.Helpers;
using api.Helpers.CQRS;

namespace api.Features.Game.SendMessage;

public class SendMessageCommand: ICommand<Result<string>>
{
    public Guid? UserId { get; set; }
    public Guid GameId { get; set; }
    public string Message { get; set; }
}