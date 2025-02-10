using api.DataAccess;
using api.Helpers;
using api.Services.Jwt;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace api.Features.Auth.Login;

public class UserLoginCommandHandler: IRequestHandler<UserLoginCommand, Result<string>>
{
    private readonly IJwtGenerator _jwtGenerator;
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher<object> _passwordHasher;

    public UserLoginCommandHandler(IJwtGenerator jwtGenerator, AppDbContext dbContext, IPasswordHasher<object> passwordHasher)
    {
        _jwtGenerator = jwtGenerator;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<string>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Username == request.Username);

        if (user is null)
        {
            return Result<string>.Failure(StatusCodes.Status400BadRequest, "Такого юзера нет или пароль неверный");
        }

        if (_passwordHasher.VerifyHashedPassword(null, user.Password, request.Password) == PasswordVerificationResult.Failed)
        {
            return Result<string>.Failure(StatusCodes.Status401Unauthorized, "Такого юзера нет или пароль неверный");
        }

        var token = _jwtGenerator.GenerateToken(user);

        return Result<string>.Success(token);
    }
}