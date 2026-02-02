using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Configuration;

[ExcludeFromCodeCoverage]
public static class JwtTokenConfiguration
{
    private const string AUTHEN101_AUTHENTICATION_SCHEMA = "AUTHEN101";

    public static class FunctionRequire
    {
        public const string VIEW = "VIEW";
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(AUTHEN101_AUTHENTICATION_SCHEMA, options =>
        {
            var publicKey = File.ReadAllText("public.key");
            var rsa = RSA.Create();
            rsa.ImportFromPem(publicKey.ToCharArray());

            options.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new RsaSecurityKey(rsa),
                ValidateIssuer = true,
                ValidIssuer = "sme-stock-identity",
                ValidateAudience = true,
                ValidAudience = "sme-stock-domain-service",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Cookies["access_token"];
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorizationBuilder().AddPolicy(FunctionRequire.VIEW, policy =>
        {
            policy.AuthenticationSchemes.Add(AUTHEN101_AUTHENTICATION_SCHEMA);
            policy.RequireAssertion(ctx =>
                ctx.User.HasClaim(c => c.Type == "permissions" &&
                                       string.Equals(c.Value, "view_data", StringComparison.OrdinalIgnoreCase)));
        });

    }
}