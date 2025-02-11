using api.Contracts;
using api.Features.Game.GetMessages;
using Microsoft.AspNetCore.SignalR;

namespace api.Hubs;

public interface IGameHubClient
{
    Task MessageReceive(ChatMessageDto message);
    Task ResultReceive(RoundFinishDto finishGameDto);
    Task SuccessJoin(JoinRole joinRole);
    Task JoinRefused(string message);
    Task StartGame();
    Task SomethingWentWrong(ErrorDto error);
    Task AnotherPlayerMadeMove();
}
public class RoundFinishDto
{
    public string WinnerName { get; set; }
    public string LoserName { get; set; }
    public Figure WinnerFigure { get; set; }
    public Figure LoserFigure { get; set; }
    public string Message { get; set; }
    
    public bool IsDraw { get; set; }
}

public class ErrorDto
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
}