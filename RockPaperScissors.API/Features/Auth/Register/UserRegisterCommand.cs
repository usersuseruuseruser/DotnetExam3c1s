using System.ComponentModel.DataAnnotations;
using api.Helpers;
using MediatR;

namespace api.Features.Auth.Register;

public class UserRegisterCommand: IRequest<Result<string>>
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string PasswordConfirmation { get; set; }
}