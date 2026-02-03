using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Dto;
namespace Shared.Extensions;

[ExcludeFromCodeCoverage]
public static class RouteGroupBuilderExtensions
{
    public static RouteHandlerBuilder MapPostWithCommonResponses(
        this RouteGroupBuilder group,
        string pattern,
        Delegate handler)
    {
        return group.MapPost(pattern, handler)
                    .Produces<ProblemBadRequestObjectResponse>(StatusCodes.Status400BadRequest)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status401Unauthorized)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status403Forbidden)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status409Conflict)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status500InternalServerError)
                    .WithOpenApi();
    }

    public static RouteHandlerBuilder MapPutWithCommonResponses(
    this RouteGroupBuilder group,
    string pattern,
    Delegate handler)
    {
        return group.MapPut(pattern, handler)
                    .Produces<ProblemBadRequestObjectResponse>(StatusCodes.Status400BadRequest)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status401Unauthorized)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status403Forbidden)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status404NotFound)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status409Conflict)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status500InternalServerError)
                    .WithOpenApi();
    }

    public static RouteHandlerBuilder MapPatchWithCommonResponses(
    this RouteGroupBuilder group,
    string pattern,
    Delegate handler)
    {
        return group.MapPatch(pattern, handler)
                    .Produces<ProblemBadRequestObjectResponse>(StatusCodes.Status400BadRequest)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status401Unauthorized)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status403Forbidden)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status404NotFound)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status409Conflict)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status500InternalServerError)
                    .WithOpenApi();
    }

    public static RouteHandlerBuilder MapListWithCommonResponses(
    this RouteGroupBuilder group,
    string pattern,
    Delegate handler)
    {
        return group.MapGet(pattern, handler)
                    .Produces<ProblemBadRequestObjectResponse>(StatusCodes.Status400BadRequest)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status401Unauthorized)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status403Forbidden)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status500InternalServerError)
                    .WithOpenApi();
    }

    public static RouteHandlerBuilder MapGetWithCommonResponses(
    this RouteGroupBuilder group,
    string pattern,
    Delegate handler)
    {
        return group.MapGet(pattern, handler)
                    .Produces<ProblemBadRequestObjectResponse>(StatusCodes.Status400BadRequest)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status401Unauthorized)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status403Forbidden)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status404NotFound)
                    .Produces<ProblemObjectResponse>(StatusCodes.Status500InternalServerError)
                    .WithOpenApi();
    }

}