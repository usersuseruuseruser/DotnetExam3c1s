namespace api.Domain;

public class ChatMessage
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public Guid? UserId { get; set; }
    public User? User { get; set; }
    public Guid GameId { get; set; }
    public Game Game { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsSystemMessage() => UserId == null && User == null;
}