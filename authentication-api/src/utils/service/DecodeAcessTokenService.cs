
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Shared.Utils;

[ExcludeFromCodeCoverage]
public static class DecodeAccessTokenService
{
    public static string GetUsername(this ClaimsPrincipal token)
    {
        try
        {
            return token.Claims.FirstOrDefault(x => x.Type == "username")?.Value ?? string.Empty;

        }
        catch
        {

            return string.Empty;
        }
    }

}