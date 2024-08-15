using System.IdentityModel.Tokens.Jwt;

namespace Celebratix.Common.Models.DTOs;

public class JwtTokenDto
{
    public JwtTokenDto(JwtSecurityToken token)
    {
        Token = new JwtSecurityTokenHandler().WriteToken(token);
        Expiration = token.ValidTo;
    }

    public string Token { get; set; }

    public DateTime Expiration { get; set; }
}
