using System.ComponentModel.DataAnnotations;
using api.Helpers;
using MediatR;

namespace api.Features.Auth.Login;

public class UserLoginCommand: IRequest<Result<string>>
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}