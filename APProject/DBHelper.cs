using System.Configuration;
using Npgsql;

namespace APProject
{
    /// <summary>
    /// Central database helper – returns a ready-to-use NpgsqlConnection.
    /// The connection string is read from App.config (key "PostgresDB").
    /// </summary>
    internal static class DBHelper
    {
        private static readonly string _connStr =
            ConfigurationManager.ConnectionStrings["PostgresDB"].ConnectionString;

        /// <summary>
        /// Creates and returns a new (closed) NpgsqlConnection.
        /// Call conn.Open() before using it, or use it inside a 'using' block.
        /// </summary>
        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connStr);
        }
    }
}
