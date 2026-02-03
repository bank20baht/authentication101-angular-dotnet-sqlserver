using Shared.ApplicationErrors;
using System.Text.Json;
using Shared.Dto;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;

namespace Shared.Middleware;

[ExcludeFromCodeCoverage]
public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    private const string APPLICATION_JSON = "application/json";
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await HandleNotFoundExceptionAsync(context, ex);
        }
        catch (InvalidPassword ex)
        {
            await HandleInvalidPasswordAsync(context, ex);
        }
        catch (SqlException ex)
        {
            await HandleDatabaseErrorExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleInvalidPasswordAsync(HttpContext context, InvalidPassword ex)
    {
        context.Response.ContentType = APPLICATION_JSON;
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        var error_detail = new ProblemDetailResponse
        {
            code = $"ERROR{StatusCodes.Status401Unauthorized}",
            desc = ex.Message
        };

        var error_object = JsonSerializer.Serialize(new ProblemObjectResponse
        {
            status = error_detail
        });

        return context.Response.WriteAsync(error_object);
    }

    private static Task HandleNotFoundExceptionAsync(HttpContext context, NotFoundException ex)
    {
        context.Response.ContentType = APPLICATION_JSON;
        context.Response.StatusCode = StatusCodes.Status404NotFound;

        var error_detail = new ProblemDetailResponse
        {
            code = $"ERROR{StatusCodes.Status404NotFound}",
            desc = ex.Message
        };

        var error_object = JsonSerializer.Serialize(new ProblemObjectResponse
        {
            status = error_detail
        });

        return context.Response.WriteAsync(error_object);
    }

    private static Task HandleDatabaseErrorExceptionAsync(HttpContext context, SqlException ex)
    {
        context.Response.ContentType = APPLICATION_JSON;
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var error_detail = new ProblemDetailResponse
        {
            code = $"ERROR{StatusCodes.Status500InternalServerError}",
            desc = ex.Message
        };

        var error_object = JsonSerializer.Serialize(new ProblemObjectResponse
        {
            status = error_detail
        });

        return context.Response.WriteAsync(error_object);
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = APPLICATION_JSON;
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var error_detail = new ProblemDetailResponse
        {
            code = $"ERROR{StatusCodes.Status500InternalServerError}",
            desc = ex.Message
        };

        var error_object = JsonSerializer.Serialize(new ProblemObjectResponse
        {
            status = error_detail
        });

        return context.Response.WriteAsync(error_object);
    }
}