using Npgsql;
using System.Data.Common;
using Microsoft.Extensions.Hosting;

namespace Galaxy.Conqueror.API.Configuration.Database;

public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlConnectionFactory(IConfiguration config, IHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }
        else
        {
            var host = Environment.GetEnvironmentVariable("DB_HOST");
            var username = Environment.GetEnvironmentVariable("DB_USERNAME");
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
            var database = Environment.GetEnvironmentVariable("DB_NAME");

            _connectionString = $"Host={host};Username={username};Password={password};Database={database}";
        }
    }

    public DbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
