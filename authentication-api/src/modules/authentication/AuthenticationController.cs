
namespace AuthenticationService.Controller;

using System.Text.Encodings.Web;
using System.Text.Json;
using MediatR;
using AuthenticationService.Commands;
using AuthenticationService.Dto;
using AuthenticationService.Entity;
using AuthenticationService.Query;
using Shared.ApplicationErrors;
using Shared.Extensions;
using Shared.Models;
using Shared.Utils;
using System.Security.Claims;
using static AuthenticationService.Configuration.JwtTokenConfiguration;

public static class AuthenticationController
{
    public static void UseAuthenticationController(this IEndpointRouteBuilder routes)
    {
        var authenticationRoutes = routes.MapGroup("/api/authentication").WithTags("[API] Authentication");

        authenticationRoutes.MapPostWithCommonResponses("login", async (AuthenticationRequestBodyDto body, IMediator mediator, IPasswordHasher hasher, AccessTokenService accessTokenService, RefreshTokenService refreshTokenService, HttpContext http, CancellationToken cancellationToken) =>
        {
            var existingUser = await mediator.Send(new GetUserPermissionByUsernameQuery(body.username), cancellationToken);
            if (existingUser == null)
            {
                throw new NotFoundException("plasse register");
            }

            bool verifyed = hasher.Verify(body.password, existingUser.password ?? "");
            if (!verifyed)
            {
                throw new InvalidPassword("invalid password");
            }

            List<string> permissions = JsonSerializer.Deserialize<List<string>>(existingUser.allow_function ?? "") ?? [];
            var allowFunctionJson = JsonSerializer.Serialize(permissions,
                    new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = false
                    });

            var accessToken = accessTokenService.Create(new AuthenticationModel
            {
                UserName = existingUser.username,
                Permissions = permissions,
            });

            var refreshToken = refreshTokenService.Create(Guid.NewGuid());
            await mediator.Send(new UpdateRefreshTokenByUsernameCommand(new UserPermission
            {
                username = body.username,
                refresh_token = refreshToken,
                allow_function = allowFunctionJson
            }), cancellationToken);



            http.Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(1)
            });
            http.Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(2)
            });

            return TypedResults.Ok(new AuthorizationResponseDto
            {
                username = body.username,
                application_function = permissions,
            });

        });


        authenticationRoutes.MapPostWithCommonResponses("register", async (AuthenticationRequestBodyDto body, IPasswordHasher hasher, IMediator mediator, CancellationToken cancellationToken) =>
        {
            List<string> permission = ["view_data"];

            var allowFunctionJson = JsonSerializer.Serialize(permission,
            new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            });

            var existingUser = await mediator.Send(new GetUserPermissionByUsernameQuery(body.username), cancellationToken);
            if (existingUser == null)
            {
                var hashPassword = hasher.Hash(body.password);
                await mediator.Send(new AddUserPermissionCommand(new UserPermission
                {
                    username = body.username,
                    refresh_token = null,
                    password = hashPassword,
                    allow_function = allowFunctionJson
                }), cancellationToken);
            }
            else
            {
                throw new ApplicationErrorException("existing username");
            }

            return TypedResults.Ok(new AuthorizationResponseDto
            {
                username = body.username,
                application_function = permission,
            });

        });

        authenticationRoutes.MapGetWithCommonResponses("user", async (ClaimsPrincipal token, CancellationToken cancellationToken) =>
        {
            return TypedResults.Ok(new
            {
                username = token.GetUsername()
            });
        }).RequireAuthorization(FunctionRequire.VIEW);

        authenticationRoutes.MapPostWithCommonResponses("refresh-token", async
        (
            RefreshTokenRequestBodyDto body,
            IMediator mediator,
            AccessTokenService accessTokenService,
            RefreshTokenService refreshTokenService,
            HttpContext http,
            CancellationToken cancellationToken
            ) =>
        {
            var refreshToken = http.Request.Cookies["refresh_token"] ?? "";

            var isExpired = RefreshTokenService.IsRefreshTokenExpired(refreshToken);
            if (isExpired)
            {
                throw new RefreshTokenNotSameErrorException("Session หมดอายุ กรุณาเข้าสู่ระบบใหม่");
            }

            var existingUser = await mediator.Send(new GetUserPermissionByUsernameQuery(body.username), cancellationToken);
            if (existingUser == null)
            {

                throw new NotExistingUserErrorException($"ไม่พบผู้ใช้ {body.username} ในระบบ กรุณาสมัครบัญชีก่อน");

            }

            List<string> permissions = JsonSerializer.Deserialize<List<string>>(existingUser.allow_function ?? "") ?? [];
            var accessToken = accessTokenService.Create(new AuthenticationModel
            {
                UserName = body.username,
                Permissions = permissions
            });

            http.Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(1)
            });

            return TypedResults.Ok(new AuthorizationResponseDto
            {
                username = existingUser.username,
                application_function = permissions,
            });

        });


        authenticationRoutes.MapPostWithCommonResponses("logout", (HttpContext http) =>
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            http.Response.Cookies.Append("access_token", "", cookieOptions);
            http.Response.Cookies.Append("refresh_token", "", cookieOptions);

            return TypedResults.Ok(new LogoutResponseBodyDto
            {
                message = "Logout Successful",
                timestamps = DateTime.UtcNow
            });
        })
        .WithDescription("Logout and clear authentication cookies")
        .WithSummary("Logout");
    }
}