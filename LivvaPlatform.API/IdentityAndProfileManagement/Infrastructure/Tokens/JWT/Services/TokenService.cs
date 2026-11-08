using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LivvaPlatform.API.IdentityAndProfileManagement.Application.Internal.OutboundServices;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.IdentityAndProfileManagement.Infrastructure.Tokens.JWT.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Infrastructure.Tokens.JWT.Services;

/// <summary>
///     Issues HMAC-SHA256 signed JSON Web Tokens.
/// </summary>
/// <param name="options">The token settings.</param>
public class TokenService(IOptions<TokenSettings> options) : ITokenService
{
    /// <inheritdoc />
    public string GenerateToken(User user)
    {
        var settings = options.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            settings.Issuer,
            settings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(settings.ExpirationMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}