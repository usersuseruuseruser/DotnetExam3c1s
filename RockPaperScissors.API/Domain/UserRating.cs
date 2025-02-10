using System.ComponentModel.DataAnnotations;

namespace api.Domain;

public class UserRating
{
    [Key]
    public Guid UserId { get; set; }
    public int Rating { get; set; }
}