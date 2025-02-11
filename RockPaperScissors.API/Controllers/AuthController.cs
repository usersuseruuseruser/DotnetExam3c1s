using api.Features.Auth;
using api.Features.Auth.Login;
using api.Features.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
public class AuthController: ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("/register")]
    public async Task<IActionResult> RegisterAsync(UserRegisterCommand userRegisterCommand, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(userRegisterCommand, cancellationToken);
        
        return result.IsSuccess ? Ok() : StatusCode(result.StatusCode!.Value);
    }

    [HttpPost("/login")]
    public async Task<IActionResult> LoginAsync(UserLoginCommand userLoginCommand)
    {
        var result = await _mediator.Send(userLoginCommand);
        
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode!.Value);
    }
}