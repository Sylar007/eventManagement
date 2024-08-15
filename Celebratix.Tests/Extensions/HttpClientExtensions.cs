using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Tests.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace Celebratix.Tests.Extensions;

public static class HttpClientExtensions
{
    public static HttpClient SetUserClaims(this HttpClient client, ApplicationUser user, Enums.Role role) =>
        client.SetClientAuthentication(
            new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", role.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id));

    public static async Task<TResponse?> DeserializeResponseContent<TResponse>(this HttpContent response) =>
        JsonSerializer.Deserialize<TResponse>(
            await response.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

    private static HttpClient SetClientAuthentication(this HttpClient client,
        params Claim[] claims)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme,
                LocalJwtToken.GenerateToken(claims));

        return client;
    }
}