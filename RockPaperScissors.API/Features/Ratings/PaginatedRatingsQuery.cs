using System.ComponentModel.DataAnnotations;
using api.Helpers;
using api.Helpers.CQRS;

namespace api.Features.Ratings;

public class PaginatedRatingsQuery: IQuery<PaginationWrapper<UserRatingDto>>
{
    [Range(0, int.MaxValue, ErrorMessage = "Страница должна быть больше или равно нулю")]
    [Required]
    public int Page { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Число записей на странице должно быть больше нуля")]
    [Required]
    public int Count { get; set; }
}