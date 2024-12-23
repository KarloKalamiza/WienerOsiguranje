using System.Data;

namespace Backend.DataAccess.DbConnection;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
