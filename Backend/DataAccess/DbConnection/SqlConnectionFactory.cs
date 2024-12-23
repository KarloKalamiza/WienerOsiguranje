﻿using Microsoft.Data.SqlClient;
using System.Data;

namespace Backend.DataAccess.DbConnection;

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }
    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
