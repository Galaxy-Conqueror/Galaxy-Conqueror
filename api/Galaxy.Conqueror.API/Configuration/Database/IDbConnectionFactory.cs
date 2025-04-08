using System.Data;

namespace Galaxy.Conqueror.API.Configuration.Database;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
