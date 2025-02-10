using System.Security.Claims;
using api.Features.Game.Create;
using api.Features.Game.GetPaginated;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController()]
[Route("game")]
[Authorize]
public class GameController: ControllerBase
{
    private readonly IMediator _mediator;

    public GameController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateGameCommand command)
    {
        var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return BadRequest("Нужен id юзера в токене");
        }
        
        command.UserId = new Guid(userId);

        var result = await _mediator.Send(command);

        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode!.Value, result.ErrorMessage);
    }

    [HttpGet("/all")]
    public async Task<IActionResult> GetPaginatedGames([FromQuery]GetPaginatedGamesQuery query)
    {
        var result = await _mediator.Send(query);
        
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode!.Value, result.ErrorMessage);
    }
}