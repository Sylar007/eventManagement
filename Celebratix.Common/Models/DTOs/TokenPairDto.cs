using System.IdentityModel.Tokens.Jwt;

namespace Celebratix.Common.Models.DTOs;

public class TokenPairDto
{
    public TokenPairDto(JwtSecurityToken token, JwtSecurityToken refreshToken)
    {
        Token = new JwtTokenDto(token);
        RefreshToken = new JwtTokenDto(refreshToken);
    }

    public JwtTokenDto Token { get; set; }
    public JwtTokenDto RefreshToken { get; set; }
}

