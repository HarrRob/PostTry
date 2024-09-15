using System;
using System.Data.OleDb;

namespace AccessDatabaseExample
{
    public class AccessDatabase
    {
        private readonly string _connectionString;

        public AccessDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveAuthorizationCode(string authorizationCode, DateTime timestamp)
        {
            using (var conn = new OleDbConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Connection to the database successful.");

                    string insertQuery = "INSERT INTO AccessCodes (AccessCode, Timestamp) VALUES (?, ?)";
                    using (var cmd = new OleDbCommand(insertQuery, conn))
                    {
                        // Add parameters to prevent SQL injection
                        cmd.Parameters.AddWithValue("?", authorizationCode);
                        cmd.Parameters.AddWithValue("?", timestamp);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"Rows affected: {rowsAffected}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to save data: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}
