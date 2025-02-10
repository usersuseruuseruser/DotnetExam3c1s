namespace api.Domain;

public class Game
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatorId { get; set; }
    public User Creator { get; set; }
    public Guid? SecondPlayerId { get; set; }
    public User? SecondPlayer { get; set; }
    public int MaxRating { get; set; }
    public GameStatus GameStatus { get; set; }
}

public enum GameStatus
{
    Created,
    Started,
    Finished
}