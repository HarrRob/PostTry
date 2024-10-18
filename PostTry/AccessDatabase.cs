using Microsoft.Data.Sqlite;
using System;

namespace SpotifyAuthExample
{
    public class SQLiteDatabase
    {
        private readonly string _connectionString;

        public SQLiteDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveUserInfo(string firstName, string lastName, string authCode)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    var insertCommand = connection.CreateCommand();
                    insertCommand.CommandText =
                    @"
                        INSERT INTO Users (FirstName, LastName, AuthCode)
                        VALUES ($firstName, $lastName, $authCode);
                    ";
                    insertCommand.Parameters.AddWithValue("$firstName", firstName);
                    insertCommand.Parameters.AddWithValue("$lastName", lastName);
                    insertCommand.Parameters.AddWithValue("$authCode", authCode);
                    insertCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public void DisplayUsers()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                string query = "SELECT * FROM Users";
                using (var command = new SqliteCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"UserID: {reader["UserID"]}, FirstName: {reader["FirstName"]}, LastName: {reader["LastName"]}, AUTHCODE: {reader["AuthCode"]}, CreatedAt: {reader["CreatedAt"]}");
                            Console.ReadKey();
                        }
                    }
                }
            }
        }
    }
}
