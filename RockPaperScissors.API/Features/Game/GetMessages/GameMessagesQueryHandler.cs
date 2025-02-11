using api.DataAccess;
using api.Helpers;
using api.Helpers.CQRS;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Game.GetMessages;

public class GameMessagesQueryHandler: IQueryHandler<GameMessagesQuery, Result<GameMessagesDto>>
{
    private readonly AppDbContext _dbContext;

    public GameMessagesQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<GameMessagesDto>> Handle(GameMessagesQuery request, CancellationToken cancellationToken)
    {
        var game = _dbContext
            .Games
            .Include(g => g.Messages)
            .ThenInclude(m => m.User)
            .FirstOrDefault(g => g.Id == request.GameId);
        
        if (game == null)
        {
            return Result<GameMessagesDto>.Failure(StatusCodes.Status400BadRequest, "Такой игры нет");
        }

        var dtoMessagesSorted = 
            game
                .Messages
                .OrderBy(m => m.CreatedAt)
                .Select(m => new ChatMessageDto()
                {
                    From = m.IsSystemMessage()? "Игра": m.User!.Username,
                    Text = m.Text
                })
                .ToList();

        var dto = new GameMessagesDto()
        {
            Messages = dtoMessagesSorted
        };

        return Result<GameMessagesDto>.Success(dto);
    }
}