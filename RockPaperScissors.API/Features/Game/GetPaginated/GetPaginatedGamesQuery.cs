using System.ComponentModel.DataAnnotations;
using api.Features.Game.Create;
using api.Helpers;
using api.Helpers.CQRS;

namespace api.Features.Game.GetPaginated;

public class GetPaginatedGamesQuery: IQuery<Result<PaginationWrapper<GameDto>>>
{
    [Range(0, int.MaxValue, ErrorMessage = "Страница должна быть больше или равно нулю")]
    [Required]
    public int Page { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Число записей на странице должно быть больше нуля")]
    [Required]
    public int Count { get; set; }
}