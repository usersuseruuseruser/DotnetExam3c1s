using api.Features.Ratings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Authorize]
public class RatingsController: ControllerBase
{
    private readonly IMediator _mediator;

    public RatingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("/ratings")]
    public async Task<IActionResult> Get([FromQuery] PaginatedRatingsQuery query)
    {
        var res = await _mediator.Send(query);
        
        return Ok(res);
    }
}