namespace api.Contracts;

public record UpdateUsersRatings(UpdateInfo user1, UpdateInfo user2);
public class UpdateInfo
{
    public Guid UserId { get; set; }
    public int Delta { get; set; }
}