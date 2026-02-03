using Microsoft.Data.SqlClient;

namespace AuthenticationService.DatabaseContext;

public interface ISqlConnectionFactory
{
    SqlConnection CreateConnection();
}