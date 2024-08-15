using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Celebratix.Common.Configs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
// ReSharper disable ArgumentsStyleNamedExpression

namespace Celebratix.Common.Services;

public class JwtService
{
    private readonly JwtBearerConfig _config;
    private readonly SymmetricSecurityKey _authSigningKey;

    public JwtService(IOptions<JwtBearerConfig> config)
    {
        _config = config.Value;
        _authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret));
    }

    public JwtSecurityToken GenerateToken(List<Claim> authClaims, DateTime expires)
    {
        var signingCredentials = new SigningCredentials(_authSigningKey, SecurityAlgorithms.HmacSha256);
        return new JwtSecurityToken(
            issuer: _config.Issuer,
            audience: _config.Audience,
            expires: expires,
            claims: authClaims,
            signingCredentials: signingCredentials
        );
    }

    public bool IsValidToken(string token)
    {
        var validationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = _authSigningKey,
            ValidIssuer = _config.Issuer,
            ValidAudience = _config.Audience,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
        };
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token))
        {
            return false;
        }
        SecurityToken securityToken;
        try
        {
            var principal = handler.ValidateToken(token, validationParameters, out securityToken);
            return true;
        }
        catch (SecurityTokenException)
        {
            return false;
        }
    }
}
