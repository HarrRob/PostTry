using System;
using System.Data.OleDb;

namespace SpotifyAuthExample
{
    public class AccessDatabase
    {
        private readonly string _connectionString;

        public AccessDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveUserInfo(string firstName, string lastName, string authCode)
        {
            using (OleDbConnection connection = new OleDbConnection(_connectionString))
            {
                string query = "INSERT INTO Users (FirstName, LastName, AUTHCODE, CreatedAt) VALUES (@FirstName, @LastName, @AuthCode, @CreatedAt)";

                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@AuthCode", authCode);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        public void DisplayUsers()
        {
            using (OleDbConnection connection = new OleDbConnection(_connectionString))
            {
                string query = "SELECT * FROM Users";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    connection.Open();
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"UserID: {reader["UserID"]}, FirstName: {reader["FirstName"]}, LastName: {reader["LastName"]}, AUTHCODE: {reader["AUTHCODE"]}, CreatedAt: {reader["CreatedAt"]}");
                        }
                    }
                    connection.Close();
                }
            }
        }
    }
}

