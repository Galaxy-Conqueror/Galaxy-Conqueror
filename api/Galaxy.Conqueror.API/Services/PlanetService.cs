﻿using System.Data.Common;
using Dapper;
using Galaxy.Conqueror.API.Configuration.Database;
using Galaxy.Conqueror.API.Models.Database;

namespace Galaxy.Conqueror.API.Services;

public class PlanetService(IDbConnectionFactory connectionFactory)
{
    public async Task<IEnumerable<Planet>> GetPlanets()
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM planets";
        return await connection.QueryAsync<Planet>(sql);
    }

    public async Task<Planet?> GetPlanetById(int id)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM planets WHERE id = @Id";
        return await connection.QuerySingleOrDefaultAsync<Planet>(sql, new { Id = id });
    }

    public async Task<Planet?> GetPlanetByUserID(Guid userId)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM planets WHERE user_id = @UserId";
        return await connection.QuerySingleOrDefaultAsync<Planet>(sql, new { UserId = userId });
    }

    public async Task<Planet?> GetPlanetBySpaceshipId(int spaceshipId)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "SELECT p.* FROM planets p JOIN spaceships s ON p.user_id = s.user_id WHERE s.id = @SpaceshipId";
        return await connection.QuerySingleOrDefaultAsync<Planet>(sql, new { SpaceshipId = spaceshipId });
    }

    public async Task<Planet> CreatePlanet(Guid userId, DbTransaction? transaction = null)
    {
        var connection = transaction?.Connection;
        if (connection == null)
            connection = connectionFactory.CreateConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        const string sql = @"
            INSERT INTO planets (user_id)
            VALUES (@UserId)
            RETURNING *;
        ";

        var planet = await connection.QuerySingleAsync<Planet>(
            sql,
            new { UserId = userId },
            transaction: transaction
        );

        if (transaction == null)
            await connection.DisposeAsync();

        return planet;
    }

    public async Task<Planet?> UpdatePlanetName(Guid userId, string newName)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE planets
            SET name = @Name
            WHERE user_id = @UserId
            RETURNING *;
        ";

        return await connection.QuerySingleOrDefaultAsync<Planet>(sql, new
        {
            Name = newName,
            UserId = userId
        });
    }

}
