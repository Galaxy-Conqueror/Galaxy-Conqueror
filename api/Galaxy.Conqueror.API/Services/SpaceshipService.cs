using System.Data.Common;
using Dapper;
using Galaxy.Conqueror.API.Configuration.Database;
using Galaxy.Conqueror.API.Models.Database;
using Galaxy.Conqueror.API.Utils;

namespace Galaxy.Conqueror.API.Services;

public class SpaceshipService(IDbConnectionFactory connectionFactory)
{
    public async Task<Spaceship?> GetSpaceshipByUserId(Guid userId)
    {
        using var connection = connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM spaceships WHERE user_id = @UserId";
        return await connection.QuerySingleOrDefaultAsync<Spaceship>(sql, new { UserId = userId });
    }

    public async Task<Spaceship> CreateSpaceship(Guid userId, Planet planet, IAiService aiService, DbTransaction? transaction = null)
    {
        string design = await aiService.AiGeneratorAsync("give me detailed ascii art making use of various characters that show a futuristic spaceship facing towards the top of the screen, it must look like a fighter spaceship and is strictly limited to 10 wide (left to right) and 6 high (up down) use special characters like ╔, ╬, ═ for weapons and sparingly Use six▼ for thrusters. and ▓ for armor. I want it to have as much detail as possible. And include a single prominent weapon on the center column protrudes beyond the rest of the ship upwards, try and incorporate as much detail as possible using as many characters as you can, the ship must be symmetrical. In the center of the ships mass must be a cockpit. Use as many different characters as possible to add greebles. do not provide any other text or comment just the ascii.  Instead of using spaces to align everything use the character \"S\". Limit the characters you use to those found in standard windows fonts. Try and keep the ship wide like a space fighter. \n\nHere is an example of a good ship: SSSS△SSSS\\r\\nSS◣▓╬▓◢SS\\r\\nS◤╔▒╬▒╗◥S\\r\\n▲▓▓◙█◙▓▓▲\\r\\n█╗▓▓╬▓▓╔█\\r\\nSS▼▼▼▼▼▼SS\n\nVariate on this basic shape. It must be noticably different than this\n", 500);
        design = design.Length > 255 ? design[..255] : design;

        var connection = transaction?.Connection;
        if (connection == null)
            connection = connectionFactory.CreateConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        const string sql = @"
            INSERT INTO spaceships (user_id, design, description, level, current_fuel, current_health, resource_reserve, x, y)
            VALUES (@UserId, @Design, @Description, @Level, @CurrentFuel, @CurrentHealth, @ResourceReserve, @X, @Y)
            RETURNING *";
        var spaceship = new Spaceship()
        {
            UserId = userId,
            Design = design ?? "SSSS║SSSS\nSS◢▓╬▓◣SS\nS╔▓░♦░▓╗S\n◄▓▒█▣█▒▓►\n╠▓▓▲╬▲▓▓╣\nSS▼▼▼▼▼▼SS",
            Description = "This spaceship can shoot things",
            Level = 1,
            CurrentFuel = Calculations.GetSpaceshipMaxFuel(1),
            CurrentHealth = Calculations.GetSpaceshipMaxHealth(1),
            ResourceReserve = 0,
            X = planet.X,
            Y = planet.Y + 1,
        };

        var spaceshipResponse = await connection.QuerySingleAsync<Spaceship>(sql, spaceship, transaction: transaction);

        if (transaction == null)
            await connection.DisposeAsync();

        return spaceshipResponse;
    }

    public async Task<Spaceship> UpdateSpaceship(int spaceshipId, int planetId, int cost)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        try
        {
            const string updatePlanetSql = @"
                UPDATE planets
                SET resource_reserve = resource_reserve - @Cost
                WHERE id = @Id
            ";

            var affectedRows = await connection.ExecuteAsync(updatePlanetSql, new { Id = planetId, Cost = cost }, transaction);
            if (affectedRows == 0)
                throw new Exception("Insufficient resources or planet not found.");

            const string updateSpaceshipSql = @"
                UPDATE spaceships
                SET level = level + 1
                WHERE id = @Id
                RETURNING *;
            ";

            var upgradedSpaceship = await connection.QuerySingleOrDefaultAsync<Spaceship>(updateSpaceshipSql, new { Id = spaceshipId }, transaction);
            if (upgradedSpaceship == null)
                throw new Exception("Spaceship not found or update failed.");

            await transaction.CommitAsync();

            return upgradedSpaceship;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Spaceship> ResetSpaceship(int spaceshipId, Planet planet, DbTransaction? transaction = null)
    {
        var connection = transaction?.Connection;
        if (connection == null)
            connection = connectionFactory.CreateConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        const string resetSpaceshipSql = @"
            UPDATE spaceships SET
                Level = 1,
                current_fuel = 0,
                current_health = 1,
                resource_reserve = 0,
                X = @PlanetX,
                Y = @PlanetY
                WHERE spaceships.Id = @Id
                RETURNING *;
        ";

        var resetSpaceship = await connection.QuerySingleOrDefaultAsync<Spaceship>(resetSpaceshipSql, new { Id = spaceshipId, PlanetX = planet.X, PlanetY = planet.Y }, transaction);
        if (resetSpaceship == null)
            throw new Exception("Spaceship reset failed.");

        if (transaction == null)
            await connection.DisposeAsync();

        return resetSpaceship;
    }

    public async Task<Spaceship> LootResources(int spaceshipId, int planetId, int resourcesLooted, DbTransaction? transaction = null)
    {
        var connection = transaction?.Connection;
        if (connection == null)
            connection = connectionFactory.CreateConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        const string lootFromPlanetSql = @"
            UPDATE planets
            SET resource_reserve = resource_reserve - @ResourcesLooted
            WHERE id = @Id
        ";

        var affectedRows = await connection.ExecuteAsync(lootFromPlanetSql, new { Id = planetId, ResourcesLooted = resourcesLooted }, transaction);
        if (affectedRows == 0)
            throw new Exception("Insufficient resources or planet not found.");

        const string lootToSpaceshipSql = @"
            UPDATE spaceships
            SET resource_reserve = resource_reserve + @ResourcesLooted
            WHERE id = @Id
            RETURNING *;
        ";

        var spaceshipWithLoot = await connection.QuerySingleOrDefaultAsync<Spaceship>(lootToSpaceshipSql, new { Id = spaceshipId, ResourcesLooted = resourcesLooted }, transaction: transaction);
        
        if (spaceshipWithLoot == null)
            throw new Exception("Spaceship not found or update failed.");

        if (transaction == null)
            await connection.DisposeAsync();

        return spaceshipWithLoot;
    }

    public async Task<Spaceship> UpdateSpaceshipHealth(int spaceshipId, int damageToSpaceship, DbTransaction? transaction = null)
    {
        var connection = transaction?.Connection;
        if (connection == null)
            connection = connectionFactory.CreateConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        const string updateSpaceshipSql = @"
            UPDATE spaceships
            SET current_health = current_health - @DamageToSpaceship
            WHERE id = @Id
            RETURNING *;
        ";

        var updatedSpaceship = await connection.QuerySingleOrDefaultAsync<Spaceship>(updateSpaceshipSql, new { Id = spaceshipId, DamageToSpaceship = damageToSpaceship }, transaction: transaction);
        if (updatedSpaceship == null)
            throw new Exception("Spaceship not found or update failed.");

        if (transaction == null)
            await connection.DisposeAsync();

        return updatedSpaceship;
    }

    public async Task<Spaceship> Refuel (int spaceshipId, int planetId, int cost, int fuelAmount)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        try
        {
            const string updatePlanetSql = @"
                UPDATE planets
                SET resource_reserve = resource_reserve - @Cost
                WHERE id = @Id
            ";

            var affectedRows = await connection.ExecuteAsync(updatePlanetSql, new { Id = planetId, Cost = cost }, transaction);
            if (affectedRows == 0)
                throw new Exception("Insufficient resources or planet not found.");

            const string refuelSpaceshipSql = @"
                UPDATE spaceships
                SET current_fuel = current_fuel + @FuelAmount
                WHERE id = @Id
                RETURNING *;
            ";

            var refueledSpaceship = await connection.QuerySingleOrDefaultAsync<Spaceship>(refuelSpaceshipSql, new { Id = spaceshipId , FuelAmount = fuelAmount}, transaction);
            if (refueledSpaceship == null)
                throw new Exception("Spaceship not found or refuel failed.");

            await transaction.CommitAsync();
            return refueledSpaceship;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    public async Task<Spaceship> Repair (int spaceshipId, int planetId, int cost, int healthAmount)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        try
        {
            const string updatePlanetSql = @"
                UPDATE planets
                SET resource_reserve = resource_reserve - @Cost
                WHERE id = @Id
            ";

            var affectedRows = await connection.ExecuteAsync(updatePlanetSql, new { Id = planetId, Cost = cost }, transaction);
            if (affectedRows == 0)
                throw new Exception("Insufficient resources or planet not found.");

            const string repairSpaceshipSql = @"
                UPDATE spaceships
                SET current_health = current_health + @HealthAmount
                WHERE id = @Id
                RETURNING *;
            ";

            var repairedSpaceship = await connection.QuerySingleOrDefaultAsync<Spaceship>(repairSpaceshipSql, new { Id = spaceshipId , HealthAmount = healthAmount}, transaction);
            if (repairedSpaceship == null)
                throw new Exception("Spaceship not found or repair failed.");

            await transaction.CommitAsync();
            return repairedSpaceship;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Spaceship> MoveSpaceship(int spaceshipId, int fuelUsed, int newX, int newY)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        const string moveSpaceshipSql = @"
            UPDATE spaceships
            SET current_fuel = current_fuel - @FuelUsed,
            x = @NewX,
            y = @NewY
            WHERE id = @Id
            RETURNING *;
        ";

        var movedSpaceship = await connection.QuerySingleOrDefaultAsync<Spaceship>(moveSpaceshipSql, new { Id = spaceshipId, FuelUsed = fuelUsed, NewX = newX, NewY = newY });
        if (movedSpaceship == null)
            throw new Exception("Spaceship not found or move failed.");

        return movedSpaceship;
    }

    public async Task<Planet> Deposit (int spaceshipId, int planetId, int spaceshipResourceReserve)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        try
        {
            const string updatePlanetSql = @"
                UPDATE planets
                SET resource_reserve = resource_reserve + @SpaceshipResourceReserve
                WHERE id = @Id
                RETURNING *;
            ";

            var updatedPlanet = await connection.QuerySingleOrDefaultAsync<Planet>(updatePlanetSql, new { Id = planetId, SpaceshipResourceReserve = spaceshipResourceReserve }, transaction);
            if (updatedPlanet == null)
                throw new Exception("Planet not found or deposit failed.");

            const string updateSpaceshipSql = @"
                UPDATE spaceships
                SET resource_reserve = resource_reserve - @SpaceshipResourceReserve
                WHERE id = @Id
                RETURNING *;
            ";

            var updatedSpaceship = await connection.QuerySingleOrDefaultAsync<Spaceship>(updateSpaceshipSql, new { Id = spaceshipId, SpaceshipResourceReserve = spaceshipResourceReserve}, transaction);
            if (updatedSpaceship == null)
                throw new Exception("Spaceship not found or deposit failed.");

            await transaction.CommitAsync();
            return updatedPlanet;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

}