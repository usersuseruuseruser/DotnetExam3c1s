using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using api.Helpers;
using api.Helpers.CQRS;

namespace api.Features.Game.Create;

public class CreateGameCommand: ICommand<Result<GameDto>>
{
    [JsonIgnore]
    public Guid UserId { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Значение должно быть положительным.")]
    public int MaxRating { get; set; }
}