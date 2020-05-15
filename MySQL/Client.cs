using System;
using MySql.Data.MySqlClient;

namespace NWN.MySQL
{
    class Client
    {
        private static readonly MySqlConnection connection = new MySqlConnection();
        private static string host = Environment.GetEnvironmentVariable("NWNX_SQL_HOST");
        private static string port = Environment.GetEnvironmentVariable("NWNX_SQL_PORT");
        private static string user = Environment.GetEnvironmentVariable("NWNX_SQL_USERNAME");
        private static string password = Environment.GetEnvironmentVariable("NWNX_SQL_PASSWORD");
        private static string database = Environment.GetEnvironmentVariable("NWNX_SQL_DATABASE");

        static public void Connect ()
        {
            connection.ConnectionString =
                $"server={host};" +
                $"port={port};" +
                $"uid={user};" +
                $"pwd={password};" + 
                $"database={database}";
            connection.Open();
        }

        static public MySqlCommand CreateCommand (string query)
        {
            return new MySqlCommand(query, connection);
        }
    }
}
