using api.DataAccess;
using api.Domain;
using api.Helpers;
using api.Services.Jwt;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Auth.Register;

public class UserRegisterCommandHandler: IRequestHandler<UserRegisterCommand, Result<string>>
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher<object> _passwordHasher;

    public UserRegisterCommandHandler(AppDbContext dbContext, IPasswordHasher<object> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<string>> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
    {
        if (request.Password != request.PasswordConfirmation)
        {
            return Result<string>.Failure(StatusCodes.Status400BadRequest, "Пароли не совпадают");
        }

        var user = new User()
        {
            Username = request.Username,
            Password = _passwordHasher.HashPassword(null, request.Password)
        };

        _dbContext.Users.Add(user);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            return Result<string>.Failure(StatusCodes.Status409Conflict, "Такой юзер уже есть");
        }
        catch (Exception ex)
        {
            return Result<string>.Failure(StatusCodes.Status500InternalServerError, "Что-то пошло не так");
        }
        
        return Result<string>.Success("Вы успешно зарегистрировались");
    }

}