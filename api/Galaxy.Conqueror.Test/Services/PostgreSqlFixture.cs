using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Testcontainers.PostgreSql;
using Npgsql;
using Dapper;

public class PostgreSqlFixture : IAsyncLifetime
{
    public PostgreSqlContainer? Container { get; private set; }
    public NpgsqlConnection Connection => new(Container?.GetConnectionString());
    public async Task InitializeAsync(){
        Container = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .Build();

        await Container.StartAsync();

        var connectionString = Container.GetConnectionString();

        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

                              
        var migrationPath = @".\..\..\..\..\..\flyway\migrations\V1__init.sql";
        var setupSql = await File.ReadAllTextAsync(migrationPath);
        await conn.ExecuteAsync(setupSql);

    }

    public TestDbConnectionFactory ConnectionFactory => new(Container!.GetConnectionString());

    public async Task TruncateTables()
    {
    await using var conn = new NpgsqlConnection(Container!.GetConnectionString());
    await conn.ExecuteAsync(@"
        TRUNCATE TABLE users, planets, resource_extractors, turrets, spaceships, battles RESTART IDENTITY CASCADE;
    ");
    }


    public async Task DisposeAsync() => await Container.DisposeAsync();
}
