using api.DataAccess;
using api.Domain;
using api.Helpers;
using api.Helpers.CQRS;

namespace api.Features.Game.SendMessage;

public class SendMessageCommandHandler: ICommandHandler<SendMessageCommand, Result<string>>
{
    private readonly AppDbContext _dbContext;

    public SendMessageCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<string>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var message = new ChatMessage()
        {
            GameId = request.GameId,
            Text = request.Message,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow,
        };

        _dbContext.ChatMessages.Add(message);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<string>.Success("Сообщение отправлено");
    }
}