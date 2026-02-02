using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Shared.Models;
namespace Shared.Utils;

[ExcludeFromCodeCoverage]
public class AccessTokenService
{
    public string Create(AuthenticationModel user)
    {
        var privateKey = File.ReadAllText("private.key");
        var rsa = RSA.Create();
        rsa.ImportFromPem(privateKey.ToCharArray());

        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = signingCredentials,
            Audience = "sme-stock-domain-service",
            Issuer = "sme-stock-identity",
            Expires = DateTime.UtcNow.AddMinutes(1),
            Subject = GenerateClaims(user)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static ClaimsIdentity GenerateClaims(AuthenticationModel user)
    {
        var ci = new ClaimsIdentity();
        ci.AddClaim(new Claim("username", user.UserName.ToString()));
        foreach (var perm in user.Permissions ?? Array.Empty<string>())
        {
            ci.AddClaim(new Claim("permissions", perm));
        }
        return ci;
    }
}