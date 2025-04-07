using System.Data;
using Dapper;

namespace Galaxy.Conqueror.API.Services;

public class PlanetService(IDbConnection db)
{
    private readonly IDbConnection _db = db;

    public async Task<IEnumerable<Planet>> GetPlanets()
    {
        const string sql = "SELECT * FROM planets";
        return await _db.QueryAsync<Planet>(sql);
    }

    public async Task<Planet?> GetPlanetById(Guid id)
    {
        const string sql = "SELECT * FROM planets WHERE id = @Id";
        return await _db.QuerySingleOrDefaultAsync<Planet>(sql, new { Id = id });
    }

    public async Task<Planet?> GetPlanetByUserID(Guid userId)
    {
        const string sql = "SELECT * FROM planets WHERE userId = @UserId";
        return await _db.QuerySingleOrDefaultAsync<Planet>(sql, new { UserId = userId });
    }

    public async Task<Planet> CreatePlanet(Guid userId)
    {
        const string sql = @"
            INSERT INTO planets (user_id)
            VALUES (@UserId)
            RETURNING *";
        var planet = new Planet()
        {
            UserId = userId,
            // add defaults settings for planets (awaiting the database desing)
        };
        return await _db.QuerySingleAsync<Planet>(sql, planet);
    }


    public class Planet
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

}
