using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Domain;
using Microsoft.IdentityModel.Tokens;

namespace api.Services.Jwt;

public interface IJwtGenerator
{
    string GenerateToken(User user);
}

public class JwtGenerator : IJwtGenerator
{
    private readonly SymmetricSecurityKey _key;
    private readonly string? _audience;
    private readonly string? _issuer;

    public JwtGenerator(IConfiguration config)
    {
        _issuer = config["JwtOptions:Issuer"];
        _audience = config["JwtOptions:Audience"];
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtOptions:Key"]!));
    }
    
    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username!),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var jwt = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
            signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256));
            
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}