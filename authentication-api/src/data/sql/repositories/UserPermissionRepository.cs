using System.Diagnostics.CodeAnalysis;
using Dapper;
using Microsoft.EntityFrameworkCore;
using AuthenticationService.Entity;
using AuthenticationService.IRepository;
using Shared.ApplicationErrors;
using AuthenticationService.DatabaseContext;

namespace AuthenticationService.Repository;

[ExcludeFromCodeCoverage]
public class UserPermissionRepository(ISqlConnectionFactory connectionFactory) : IUserPermissionRepository
{
    private readonly ISqlConnectionFactory _connectionFactory = connectionFactory;
    public async Task<bool> AddUserPermissionAsync(
        UserPermission body,
        CancellationToken cancellationToken)
    {
        await using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            INSERT INTO UserPermission
            (username, refresh_token, allow_function)
            VALUES
            (@username, @refresh_token, @allow_function)
        ";

        var affected = await connection.ExecuteAsync(sql, body);

        return affected > 0;
    }


    public async Task<UserPermission?> GetUserPermissionByOaUsernameAsync(
        string oa_username,
        CancellationToken cancellationToken)
    {
        await using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT *
            FROM UserPermission
            WHERE username = @username
        ";

        return await connection.QueryFirstOrDefaultAsync<UserPermission>(
            sql,
            new { username = oa_username }
        );
    }

    public async Task<bool> UpdateRefreshTokenByUsernameAsync(UserPermission body, CancellationToken cancellationToken)
    {
        await using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE UserPermission
            SET
                refresh_token = @refresh_token,
                allow_function = @allow_function
            WHERE username = @username
        ";

        var affected = await connection.ExecuteAsync(sql, body);

        if (affected == 0)
            throw new NotFoundException("teller not found");

        return true;
    }
}