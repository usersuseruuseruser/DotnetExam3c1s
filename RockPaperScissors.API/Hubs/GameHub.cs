using System.Collections.Concurrent;
using System.Security.Claims;
using api.Contracts;
using api.Domain;
using api.Features.Game.CalculateRoundResult;
using api.Features.Game.ChangeStatus;
using api.Features.Game.GetMessages;
using api.Features.Game.GetSingle;
using api.Features.Game.Join;
using api.Features.Game.Leave;
using api.Features.Game.SendMessage;
using api.Features.Ratings;
using api.Features.User.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace api.Hubs;

[Authorize]
public class GameHub: Hub<IGameHubClient>
{
    private readonly IMediator _mediator;
    private readonly ILogger<GameHub> _logger;
    private static readonly ConcurrentDictionary<string, HashSet<string>> GameConnections = new();
    private static readonly ConcurrentDictionary<string, UserGroups> UserGroupsConnections = new();
    private static readonly ConcurrentDictionary<string, HashSet<UserMove>> UsersMove = new();

    public GameHub(IMediator mediator, ILogger<GameHub> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task JoinGame(Guid gameId)
    {
        var username = Context.User!.Identity!.Name;
        var userId = new Guid(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        var playersGroup = $"{gameId}_game";
        var viewersGroup = gameId;

        var checkUserRatingQuery = new UserRatingQuery() { GameId = gameId, UserId = userId };
        var result = await _mediator.Send(checkUserRatingQuery);

        if (!result.IsSuccess)
        {
            await Clients.Client(Context.ConnectionId).SomethingWentWrong(new ErrorDto()
            {
                StatusCode = result.StatusCode!.Value,
                Message = result.ErrorMessage!
            });
        }
        if (!result.Data)
        {
            await Clients.Client(Context.ConnectionId).JoinRefused("Рейтинг выше максимального!");
            return;
        }

        // Проверяем статус игры
        var query = new GetSingleGameQuery() { GameId = gameId };
        var qRes = await _mediator.Send(query);
        if (!qRes.IsSuccess)
        {
            await Clients.Client(Context.ConnectionId).SomethingWentWrong(new ErrorDto()
            {
                StatusCode = result.StatusCode!.Value,
                Message = result.ErrorMessage!
            });
        }
        var gameStatus = qRes.Data.Status;
        
        if (gameStatus == GameStatus.Finished)
        {
            if (!UserGroupsConnections.ContainsKey(Context.ConnectionId))
            {
                UserGroupsConnections[Context.ConnectionId] = new UserGroups();
            }
            UserGroupsConnections[Context.ConnectionId].RoomGroup = viewersGroup;

            await Groups.AddToGroupAsync(Context.ConnectionId, viewersGroup.ToString());

            await Clients.Client(Context.ConnectionId).MessageReceive(
                new ChatMessageDto()
                {
                    From = "Игра",
                    Text = $"Игрок {username} присоединился как наблюдатель"
                });
            await Clients.Client(Context.ConnectionId).SuccessJoin(JoinRole.Watcher);
            return;
        }

        if (!GameConnections.ContainsKey(playersGroup))
        {
            GameConnections[playersGroup] = new HashSet<string>();
        }

        if (GameConnections[playersGroup].Count < 2)
        {
            var newPlayer = GameConnections[playersGroup].Add(username);
            if (!newPlayer)
            {
                return;
            }

            var userJoinCommand = new JoinGameCommand() { GameId = gameId, Username = username, UserId = userId};
            var joinResult = await _mediator.Send(userJoinCommand);
            
            if (!joinResult.IsSuccess)
            {
                await Clients.Client(Context.ConnectionId).SomethingWentWrong(new ErrorDto()
                {
                    StatusCode = joinResult.StatusCode!.Value,
                    Message = joinResult.ErrorMessage!
                });
            }

            if (!UserGroupsConnections.ContainsKey(Context.ConnectionId))
            {
                UserGroupsConnections[Context.ConnectionId] = new UserGroups();
            }
            UserGroupsConnections[Context.ConnectionId].GameGroup = playersGroup;

            await Groups.AddToGroupAsync(Context.ConnectionId, playersGroup);

            if (GameConnections[playersGroup].Count == 2)
            {
                var changeGameStatusCommand = new ChangeGameStatusCommand() { GameId = gameId, Status = GameStatus.Started };
                var changeStatusResult = await _mediator.Send(changeGameStatusCommand);

                if (!changeStatusResult.IsSuccess)
                {
                    await Clients.Client(Context.ConnectionId).SomethingWentWrong(new ErrorDto()
                    {
                        StatusCode = changeStatusResult.StatusCode!.Value,
                        Message = changeStatusResult.ErrorMessage!
                    });
                }
                
                await Clients.Client(Context.ConnectionId).SuccessJoin(JoinRole.Player);
                await Clients.Groups(new[] { playersGroup, gameId.ToString() }).MessageReceive(
                    new ChatMessageDto()
                    {
                        From = "Игра",
                        Text = $"Игрок {username} присоединился"
                    });

                await Clients.Group(playersGroup).StartGame();
                await Clients.Client(Context.ConnectionId).MessageReceive(
                    new ChatMessageDto()
                    {
                        From = "Игра",
                        Text = $"Игра началась"
                    });

                return;
            }
            await Clients.Client(Context.ConnectionId).SuccessJoin(JoinRole.Player);
            await Clients.Client(Context.ConnectionId).MessageReceive(
                new ChatMessageDto()
                {
                    From = "Игра",
                    Text = $"Игрок {username} присоединился"
                });
        }
        else
        {
            if (!UserGroupsConnections.ContainsKey(Context.ConnectionId))
            {
                UserGroupsConnections[Context.ConnectionId] = new UserGroups();
            }
            UserGroupsConnections[Context.ConnectionId].RoomGroup = viewersGroup;

            await Groups.AddToGroupAsync(Context.ConnectionId, viewersGroup.ToString());

            await Clients.Client(Context.ConnectionId).SuccessJoin(JoinRole.Watcher);
            await Clients.Groups(new[] { playersGroup, gameId.ToString() }).MessageReceive(
                new ChatMessageDto()
                {
                    From = "Игра",
                    Text = $"Игрок {username} присоединился как наблюдатель"
                });
        }
    }

    public async Task RestartGame(Guid gameId)
    {
        var query = new GetSingleGameQuery() { GameId = gameId };
        var gameRes = await _mediator.Send(query);
        
        if (!gameRes.IsSuccess)
        {
            await Clients.Client(Context.ConnectionId).SomethingWentWrong(new ErrorDto()
            {
                StatusCode = gameRes.StatusCode!.Value,
                Message = gameRes.ErrorMessage!
            });
        }
        
        var gameStatus = gameRes.Data!.Status;
        
        var playersGroup = $"{gameId}_game";

        if (gameStatus == GameStatus.Created ||
            !GameConnections.ContainsKey(playersGroup) ||
            GameConnections[playersGroup].Count != 2)
        {
            return;
        }

        var changeGameStatusCommand = new ChangeGameStatusCommand() { GameId = gameId, Status = GameStatus.Started };
        var result = await _mediator.Send(changeGameStatusCommand);

        if (!result.IsSuccess)
        {
            await Clients.Client(Context.ConnectionId).SomethingWentWrong(new ErrorDto()
            {
                StatusCode = result.StatusCode!.Value,
                Message = result.ErrorMessage!
            });
        }
        
        await Clients.Client(Context.ConnectionId).StartGame();
    }

    
    public async Task MakeMove(Guid gameId, Figure figure)
    {
        if (!UserGroupsConnections.TryGetValue(Context.ConnectionId, out var groups) ||
            string.IsNullOrEmpty(groups.GameGroup))
        {
            await Clients.Client(Context.ConnectionId).SomethingWentWrong(new ErrorDto
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Наблюдателям нельзя делать ход."
            });
            return;
        }
        
        var username = Context.User!.Identity!.Name;
        var userId = new Guid(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var key = gameId.ToString();
        var playersGroup = $"{gameId}_game";
        
        
        UsersMove.AddOrUpdate(
            key,
            new HashSet<UserMove> { new UserMove(userId, username, figure) },
            (_, existingSet) =>
            {
                existingSet.Add(new UserMove(userId, username, figure));
                return existingSet;
            });

        if (!UsersMove.ContainsKey(key) || UsersMove[key].Count != 2)
        {
            if (UsersMove.ContainsKey(key) && UsersMove[key].Count == 1)
            {
                var currentMove = UsersMove[key].First();
                await Clients.GroupExcept(playersGroup, new List<string> { Context.ConnectionId })
                    .AnotherPlayerMadeMove();
                await  Clients.Group(gameId.ToString())
                    .MessageReceive(new ChatMessageDto(){From = "Игра", Text = $"Игрок {username} сделал ход"});
            }
            return;
        }

        var userMove = new UserMove(userId, username, figure);
        var anotherUserMove = UsersMove[key].First(x => x.Username != username);

        var handleMoveCommand = new CalculateRoundResultCommand()
        {
            GameId = gameId,
            UserMove1 = userMove,
            UserMove2 = anotherUserMove
        };

        var gameResult = await _mediator.Send(handleMoveCommand);

        if (!gameResult.IsSuccess)
        {
            await Clients.Client(Context.ConnectionId).SomethingWentWrong(new ErrorDto()
            {
                StatusCode = gameResult.StatusCode!.Value,
                Message = gameResult.ErrorMessage!
            });
        }
        _logger.LogInformation("Message: {GameMessage}", gameResult.Data!.Message);

        var data = gameResult.Data!;
        var finishDto = new RoundFinishDto()
        {
            WinnerName = data.WinnerName,
            WinnerFigure = data.WinnerFigure,
            LoserName = data.LoserName,
            LoserFigure = data.LoserFigure,
            Message = data.Message,
            IsDraw = data.IsDraw
        };
        
        await Clients.Groups(new[] { playersGroup, gameId.ToString() })
            .MessageReceive(new ChatMessageDto() { From = "Игра", Text = data.Message });
        await Clients.Group(playersGroup).ResultReceive(finishDto);

        UsersMove.TryRemove(key, out _);
    }

    public async Task SendMessageToChat(Guid gameId, string message)
    {
        var userId = new Guid(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        
        var qResult = new GetSingleGameQuery() { GameId = gameId };
        var gameRes = await _mediator.Send(qResult);
        
        if (!gameRes.IsSuccess)
        {
            await Clients.Client(Context.ConnectionId).SomethingWentWrong(new ErrorDto()
            {
                StatusCode = gameRes.StatusCode!.Value,
                Message = gameRes.ErrorMessage!
            });
        }

        if (gameRes.Data.Status == GameStatus.Finished)
        {
            await Clients.Client(Context.ConnectionId).SomethingWentWrong(new ErrorDto()
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Игра завершена"
            });
            return;
        }
        
        var cResult = await _mediator.Send(new SendMessageCommand()
        {
            Message = message,
            UserId = userId,
            GameId = gameId
        });
        
        if (cResult.IsSuccess)
        {
            await Clients.Groups(new[] { gameId.ToString(), $"{gameId}_game" })
                .MessageReceive(new ChatMessageDto(){ From = username, Text = message });
        }
        else
        {
            await Clients.Client(Context.ConnectionId).SomethingWentWrong(new ErrorDto()
            {
                StatusCode = cResult.StatusCode!.Value,
                Message = cResult.ErrorMessage!
            });
        }
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        
        if (UserGroupsConnections.TryGetValue(Context.ConnectionId, out var userGroups))
        {
            var username = Context.User?.Identity?.Name!;

            if (!string.IsNullOrEmpty(userGroups.GameGroup))
            {
                var userId = new Guid(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            
                var parts = userGroups.GameGroup.Split('_');
                if (parts.Length > 0 && Guid.TryParse(parts[0], out Guid gameId))
                {
                    var userLeaveCommand = new LeaveGameCommand()
                    {
                        GameId = gameId,
                        UserId = userId
                    };
                    await _mediator.Send(userLeaveCommand);
                    
                    await Clients.Groups(new[] { userGroups.GameGroup, gameId.ToString() })
                        .MessageReceive(new ChatMessageDto { From = "Игра", Text = $"Игрок {username} вышел" });
                
                    if (GameConnections.TryGetValue(userGroups.GameGroup, out var players))
                    {
                        players.Remove(username);
                    
                        var newStatus = players.Count == 0 ? GameStatus.Finished : GameStatus.Created;
                    
                        var changeGameStatusCommand = new ChangeGameStatusCommand()
                        {
                            GameId = gameId,
                            Status = newStatus
                        };
                        await _mediator.Send(changeGameStatusCommand);
                    }
                }
            }
        
            else if (userGroups.RoomGroup != Guid.Empty)
            {
                var gameId = userGroups.RoomGroup;
                await Clients.Groups(new[] { gameId.ToString(), $"{gameId}_game" })
                    .MessageReceive(new ChatMessageDto { From = "Игра", Text = $"Наблюдатель {username} вышел" });
            }
            
            UserGroupsConnections.TryRemove(Context.ConnectionId, out _);
        }
    
        await base.OnDisconnectedAsync(exception);
    }
    
    public async Task ClearMoves(Guid gameId)
    {
        var key = gameId.ToString();
    
        UsersMove.TryRemove(key, out _);

        var playersGroup = $"{gameId}_game";

        await Clients.Groups(new[] { playersGroup, gameId.ToString() })
            .MessageReceive(new ChatMessageDto
            {
                From = "Игра",
                Text = "Раунд завершён: один из игроков не сделал ход, раунд отменён."
            });
    }

}

public class UserGroups
{
    public Guid RoomGroup { get; set; }
    public string GameGroup { get; set; }
}