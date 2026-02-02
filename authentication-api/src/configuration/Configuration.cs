using System.Diagnostics.CodeAnalysis;

namespace AuthenticationService.Configuration;

[ExcludeFromCodeCoverage]
public class Configuration
{
    public string ConnectionString { get; }

    public Configuration()
    {
        ConnectionString = Environment.GetEnvironmentVariable("MS_SQL_URL") ?? string.Empty;
    }
}