using System;
using System.Data.SQLite;
using System.IO;

namespace PostTry
{
    public static class DatabaseHelper
    {
        private static readonly string DbDirectory = @"C:\Users\harry\source\repos\project\files";
        private static readonly string DbFilePath = Path.Combine(DbDirectory, "userManagment.db");

        public static string ConnectionString { get; } = $"Data Source={DbFilePath};Version=3;";

        public static void InitializeDatabase()
        {
            try
            {
                if (!Directory.Exists(DbDirectory))
                {
                    Directory.CreateDirectory(DbDirectory);
                }

                if (!File.Exists(DbFilePath))
                {
                    SQLiteConnection.CreateFile(DbFilePath);
                }

                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    string createTableQuery = @"
                        CREATE TABLE IF NOT EXISTS Users (
                            UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                            FirstName TEXT NOT NULL,
                            LastName TEXT NOT NULL,
                            AuthCode TEXT NOT NULL,
                            CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                        );";

                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize database: {ex.Message}");
            }
        }

        public static void AddUserInfo(string firstName, string lastName, string authCode)
        {
            try
            {
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    string insertQuery = @"
                        INSERT INTO Users (FirstName, LastName, AuthCode) 
                        VALUES (@FirstName, @LastName, @AuthCode);";

                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", firstName);
                        command.Parameters.AddWithValue("@LastName", lastName);
                        command.Parameters.AddWithValue("@AuthCode", authCode);
                        command.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("User information added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add user information: {ex.Message}");
            }
        }
    }
}
