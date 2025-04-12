using Npgsql;
using System.Data.Common;

namespace Galaxy.Conqueror.API.Configuration.Database;

public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlConnectionFactory()
    {
        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var username = Environment.GetEnvironmentVariable("DB_USERNAME");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var database = Environment.GetEnvironmentVariable("DB_NAME");

        _connectionString = $"Host={host};Username={username};Password={password};Database={database}";
    }

    public DbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
