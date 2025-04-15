using System.Data.Common;
using Dapper;
using Galaxy.Conqueror.API.Configuration.Database;
using Galaxy.Conqueror.API.Models.Database;
using Galaxy.Conqueror.API.Models.Requests;


namespace Galaxy.Conqueror.API.Services;

public class BattleService(IDbConnectionFactory connectionFactory)
{
    public async Task<Battle?> GetBattleByBattleId(int battleId)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM battles WHERE id = @Id";
        return await connection.QuerySingleOrDefaultAsync<Battle>(sql, new { Id = battleId });
    }

    public async Task<Battle[]> GetAttacksByUserId(Guid userId)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM battles JOIN spaceships ON battles.attacker_spaceship_id = spaceships.id WHERE spaceships.user_id = @UserId";
        return await connection.QuerySingleOrDefaultAsync<Battle[]>(sql, new { UserId = userId }) ?? [];
    }

    public async Task<Battle[]> GetDefensesByUserId(Guid userId)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM battles JOIN planets ON battles.defender_planet_id = planets.id WHERE planets.user_id = @UserId";
        return await connection.QuerySingleOrDefaultAsync<Battle[]>(sql, new { UserId = userId }) ?? [];
    }

    public async Task<Battle> CreateBattle(int attackerSpaceshipId, int defenderPlanetId, BattleLogRequest battleLog, bool attackerWon, DbTransaction? transaction = null)
    {
        var connection = transaction?.Connection;
        if (connection == null)
            connection = connectionFactory.CreateConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        const string sql = @"
            INSERT INTO battles (attacker_spaceship_id, defender_planet_id, started_at, ended_at, attacker_won, damage_to_spaceship, damage_to_turret, resources_looted)
            VALUES (@AttackerSpaceshipId, @DefenderPlanetId, @StartedAt, @EndedAt, @AttackerWon, @DamageToSpaceship, @DamageToTurret, @ResourcesLooted)
            RETURNING *";

        var battle = new Battle
        {
            AttackerSpaceshipId = attackerSpaceshipId,
            DefenderPlanetId = defenderPlanetId,
            StartedAt = battleLog.StartedAt,
            EndedAt = battleLog.EndedAt,
            AttackerWon = attackerWon,
            DamageToSpaceship = battleLog.DamageToSpaceship,
            DamageToTurret = battleLog.DamageToTurret,
            ResourcesLooted = battleLog.ResourcesLooted
        };

        var battleResponse = await connection.QuerySingleAsync<Battle>(sql, battle, transaction: transaction);

        if (transaction == null)
            await connection.DisposeAsync();

        return battleResponse;
    }
}