using System;
using System.Threading.Tasks;
using SpotifyAuth;
using System.Data.OleDb;

namespace SpotifyAuthExample
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("input fist name");
            string firstName = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("input last name");
            string lastName = Console.ReadLine();
            Console.Clear();
            string clientId = "f9a18ea2ae15426a8665d4df92feb418";
            string redirectUri = "http://localhost:8888/callback";
            string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\harry\Documents\access files";
            string PKCE_verifier = "simple_verifier";


            var authURL = new AuthURL(clientId, redirectUri, PKCE_verifier);
            var localServer = new LocalServer();
            _ = localServer.StartLocalServer();

            authURL.GenerateAuthorizationURL();

            Console.WriteLine("Waiting for authorization code...");

            // Wait for a while to allow the user to authenticate
            await Task.Delay(TimeSpan.FromMinutes(1)); // Adjust delay as needed

            string authorizationCode = localServer.GetAuthorizationCode();

            var spotify = await authURL.HandleCallback(authorizationCode);
            Console.WriteLine("Spotify client is ready to use.");

            // Save the authorization code and timestamp to the database
            var db = new AccessDatabase(connectionString);
            db.SaveUserInfo(firstName, lastName, authorizationCode);

            // Display all users to verify
            db.DisplayUsers();

        }
    }
}
