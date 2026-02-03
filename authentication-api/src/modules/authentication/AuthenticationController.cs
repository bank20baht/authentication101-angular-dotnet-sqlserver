
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

public static class AuthenticationController
{
    public static void UseAuthenticationController(this IEndpointRouteBuilder routes)
    {
        var authenticationRoutes = routes.MapGroup("/api/authentication").WithTags("[API] Authentication");

        authenticationRoutes.MapPostWithCommonResponses("login", async (AuthenticationRequestBodyDto body, IMediator mediator, AccessTokenService accessTokenService, RefreshTokenService refreshTokenService, HttpContext http, CancellationToken cancellationToken) =>
        {
            List<string> permission = [];

            if (body.username == "admin")
            {
                permission = ["view_data"];
            }
            else
            {
                throw new NotExistingUserErrorException($"ไม่พบผู้ใช้ {body.username} ในระบบ ดู account ที่ใช้ได้จาก github ของ repo นี้ครับ");
            }

            var allowFunctionJson = JsonSerializer.Serialize(permission,
            new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            });

            var accessToken = accessTokenService.Create(new AuthenticationModel
            {
                UserName = body.username,
                Permissions = permission,
            });

            var refreshToken = refreshTokenService.Create(Guid.NewGuid());
            var existingUser = await mediator.Send(new GetUserPermissionByUsernameQuery(body.username), cancellationToken);
            if (existingUser == null)
            {
                await mediator.Send(new AddUserPermissionCommand(new UserPermission
                {
                    username = body.username,
                    refresh_token = refreshToken,
                    allow_function = allowFunctionJson
                }), cancellationToken);
            }
            else
            {
                await mediator.Send(new UpdateRefreshTokenByUsernameCommand(new UserPermission
                {
                    username = body.username,
                    refresh_token = refreshToken,
                    allow_function = allowFunctionJson
                }), cancellationToken);
            }


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
                application_function = permission,
            });

        });

        authenticationRoutes.MapPostWithCommonResponses("refresh-token", async
        (
            RefreshTokenRequestBodyDto body,
        IMediator mediator,
        AccessTokenService accessTokenService,
         RefreshTokenService refreshTokenService,
          HttpContext http,
          CancellationToken cancellationToken) =>
        {
            var refreshToken = http.Request.Cookies["refresh_token"] ?? "";

            // TODO: open in phase2
            var isExpired = RefreshTokenService.IsRefreshTokenExpired(refreshToken);
            if (isExpired)
            {
                throw new RefreshTokenNotSameErrorException("Session หมดอายุ กรุณาเข้าสู่ระบบใหม่");
            }

            var existingUser = await mediator.Send(new GetUserPermissionByUsernameQuery(body.username), cancellationToken);
            if (existingUser == null)
            {

                throw new NotExistingUserErrorException($"ไม่พบผู้ใช้ {body.username} ในระบบ กรุณาเข้าสู่ระบบก่อน");

            }
            // TODO: open in phase2
            // if (existingUser.refresh_token != refreshToken)
            // {
            //     throw new RefreshTokenNotSameErrorException("พบการเข้าสู่ระบบซ้อน หรือ Session Id ไม่ตรงกับปัจจุบัน");
            // }

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