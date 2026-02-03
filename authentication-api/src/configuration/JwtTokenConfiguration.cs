using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shared.Dto;

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
                ValidIssuer = "authentication101",
                ValidateAudience = true,
                ValidAudience = "authentication101user",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";

                        var error_detail = new ProblemDetailResponse
                        {
                            code = $"{context.Response.StatusCode}",
                            desc = "Invalid or missing JWT token"
                        };

                        var error_object = JsonSerializer.Serialize(error_detail);
                        await context.Response.WriteAsync(error_object);
                    },
                OnAuthenticationFailed = async context =>
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";

                    var error_detail = new ProblemDetailResponse
                    {
                        code = $"{context.Response.StatusCode}",
                        desc = "Invalid or missing JWT token"
                    };

                    var error_object = JsonSerializer.Serialize(error_detail);
                    await context.Response.WriteAsync(error_object);
                },
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