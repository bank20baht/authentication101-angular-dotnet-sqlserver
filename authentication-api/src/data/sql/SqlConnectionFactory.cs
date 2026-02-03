
using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.SqlClient;
using AuthenticationService.DatabaseContext;

namespace AuthenticationService.DatabaseContext;

[ExcludeFromCodeCoverage]
public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString = Environment.GetEnvironmentVariable("MS_SQL_URL") ?? string.Empty;
    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}