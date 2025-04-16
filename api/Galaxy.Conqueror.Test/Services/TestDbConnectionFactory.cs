using Galaxy.Conqueror.API.Configuration.Database;
using Npgsql;
using System.Data.Common;

public class TestDbConnectionFactory : IDbConnectionFactory{
    private readonly string _connectionString;

    public TestDbConnectionFactory(string connectionString){
        _connectionString = connectionString;
    }

    public DbConnection CreateConnection(){
        return new NpgsqlConnection(_connectionString);
    }
}

