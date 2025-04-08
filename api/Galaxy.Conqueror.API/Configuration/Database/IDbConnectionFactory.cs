using System.Data.Common;

namespace Galaxy.Conqueror.API.Configuration.Database;

public interface IDbConnectionFactory
{
    DbConnection CreateConnection();
}
