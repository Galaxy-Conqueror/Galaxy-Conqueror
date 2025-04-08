using Npgsql;
using System.Data.Common;

namespace Galaxy.Conqueror.API.Configuration.Database;

public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlConnectionFactory(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    public DbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
