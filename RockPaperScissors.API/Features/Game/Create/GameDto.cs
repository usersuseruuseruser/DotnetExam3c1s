using api.Domain;

namespace api.Features.Game.Create;

public class GameDto
{
    public Guid GameId { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; }
    public DateTime Date { get; set; }
    public GameStatus Status { get; set; }
    public int MaxRating { get; set; }
}