using api.Features.Game.Create;

namespace api.Domain;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public List<Guid>? GameId { get; set; }
    public List<Game>? Games { get; set; }
}