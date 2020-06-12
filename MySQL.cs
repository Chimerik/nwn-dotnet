using MySql.Data.MySqlClient;
using System;

namespace NWN
{
  class MySQL
  {
    private static string host = Environment.GetEnvironmentVariable("NWNX_SQL_HOST");
    private static string port = Environment.GetEnvironmentVariable("NWNX_SQL_PORT");
    private static string user = Environment.GetEnvironmentVariable("NWNX_SQL_USERNAME");
    private static string password = Environment.GetEnvironmentVariable("NWNX_SQL_PASSWORD");
    private static string database = Environment.GetEnvironmentVariable("NWNX_SQL_DATABASE");
    private static string connectionString = $"server={host};" +
            $"port={port};" +
            $"uid={user};" +
            $"pwd={password};" +
            $"database={database}";

    public static MySqlConnection GetConnection()
    {
      return new MySqlConnection(connectionString);
    }
  }
}
