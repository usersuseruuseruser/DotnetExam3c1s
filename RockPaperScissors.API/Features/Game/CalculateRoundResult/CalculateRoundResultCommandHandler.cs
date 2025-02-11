using api.Contracts;
using api.DataAccess;
using api.Domain;
using api.Helpers;
using api.Helpers.CQRS;
using api.Hubs;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Game.CalculateRoundResult;

public class CalculateRoundResultCommandHandler: ICommandHandler<CalculateRoundResultCommand, Result<RoundFinishDto>>
{
    private readonly AppDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public CalculateRoundResultCommandHandler(AppDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<RoundFinishDto>> Handle(CalculateRoundResultCommand request, CancellationToken cancellationToken)
    {
        var firstUserAction = request.UserMove1.Figure;
        var secondUserAction = request.UserMove2.Figure;
        var chatMessage = new ChatMessage
        {
            GameId = request.GameId,
            CreatedAt = DateTime.UtcNow
        };
        var draw = false;
        UserMove winner;
        UserMove loser;
        
        if (firstUserAction == secondUserAction)
        {
            draw = true;
            winner = request.UserMove1;
            loser = request.UserMove2;
            
            chatMessage.Text = "Ничья между игроками " + request.UserMove1.Username + ":" + firstUserAction + " и " + request.UserMove2.Username + ":" + secondUserAction;
        }
        // первый игрок победил
        else if (firstUserAction == Figure.Rock && secondUserAction == Figure.Scissors ||
                 firstUserAction == Figure.Paper && secondUserAction == Figure.Rock ||
                 firstUserAction == Figure.Scissors && secondUserAction == Figure.Paper)
        {
            var updateInfo1 = new UpdateInfo
            {
                UserId = request.UserMove1.UserId,
                Delta = 3
            };
            var updateInfo2 = new UpdateInfo
            {
                UserId = request.UserMove1.UserId,
                Delta = -1
            };
            await _publishEndpoint.Publish(new UpdateUsersRatings(updateInfo1, updateInfo2), cancellationToken);
            
            winner = request.UserMove1;
            loser = request.UserMove2;
            
            chatMessage.Text = "Игрок " + request.UserMove1.Username + ":" + firstUserAction + " победил над игроком " + request.UserMove2.Username + ":" + secondUserAction;
        }
        // второй игрок победил
        else
        {
            var updateInfo1 = new UpdateInfo
            {
                UserId = request.UserMove1.UserId,
                Delta = -1
            };
            var updateInfo2 = new UpdateInfo
            {
                UserId = request.UserMove1.UserId,
                Delta = 3
            };
            await _publishEndpoint.Publish(new UpdateUsersRatings(updateInfo1, updateInfo2), cancellationToken);
            
            winner = request.UserMove2;
            loser = request.UserMove1;
            
            chatMessage.Text = "Игрок " + request.UserMove2.Username + ":" + secondUserAction + " победил над игроком " + request.UserMove1.Username + ":" + firstUserAction;
        }
        
        _context.ChatMessages.Add(chatMessage);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<RoundFinishDto>.Success(new RoundFinishDto()
        {
            Message = chatMessage.Text,
            IsDraw = draw,
            LoserFigure = loser.Figure,
            WinnerFigure = winner.Figure,
            LoserName = loser.Username,
            WinnerName = winner.Username
        });
    }
}