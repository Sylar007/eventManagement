using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Celebratix.Tests.Authentication;

public static class LocalJwtToken
{
    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    private static readonly RandomNumberGenerator RandomNumberGenerator =
        RandomNumberGenerator.Create();

    private static readonly byte[] Key = new byte[32];

    static LocalJwtToken()
    {
        RandomNumberGenerator.GetBytes(Key);
        SecurityKey = SetSecurityKey();
        SigningCredentials = SetSigningCredentials();
    }

    public static string Audience { get; } = Guid.NewGuid().ToString();

    public static string Issuer { get; } = "https://dev-ixqafgpf.us.auth0.com";

    internal static SecurityKey SecurityKey { get; }

    private static SigningCredentials SigningCredentials { get; }

    public static string GenerateToken(IReadOnlyList<Claim> claims) =>
        TokenHandler.WriteToken(new JwtSecurityToken(
            Issuer, Audience, claims, null,
            DateTime.UtcNow.AddMinutes(20), SigningCredentials));

    private static SigningCredentials SetSigningCredentials() =>
        new(SecurityKey, SecurityAlgorithms.HmacSha256);

    private static SymmetricSecurityKey SetSecurityKey() =>
        new(Key) { KeyId = Guid.NewGuid().ToString() };
}