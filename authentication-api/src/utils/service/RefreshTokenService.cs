using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Utils;

[ExcludeFromCodeCoverage]
public class RefreshTokenService
{
    public string Create(Guid refreshTokenId)
    {
        var privateKey = File.ReadAllText("private.key");
        var rsa = RSA.Create();
        rsa.ImportFromPem(privateKey.ToCharArray());

        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = signingCredentials,
            Expires = DateTime.UtcNow.AddHours(1),
            Subject = GenerateClaims(refreshTokenId)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }


    public static bool IsRefreshTokenExpired(string refreshToken)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(refreshToken))
            return true;

        var jwtToken = handler.ReadJwtToken(refreshToken);

        var exp = jwtToken.Payload.Expiration;

        if (exp == null)
            return true;

        var expirationTime = DateTimeOffset.FromUnixTimeSeconds((long)exp);

        return expirationTime < DateTimeOffset.UtcNow;
    }


    private static ClaimsIdentity GenerateClaims(Guid id)
    {
        var ci = new ClaimsIdentity();
        ci.AddClaim(new Claim("id", id.ToString()));
        return ci;
    }
}