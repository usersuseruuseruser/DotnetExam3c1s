using System.ComponentModel.DataAnnotations;
using api.Helpers;
using api.Helpers.CQRS;
using MediatR;

namespace api.Features.Auth.Register;

public class UserRegisterCommand: ICommand<Result<string>>
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string PasswordConfirmation { get; set; }
}