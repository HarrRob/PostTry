using System;
using System.Threading.Tasks;
using AccessDatabaseExample;

namespace SpotifyAuthExample
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            string clientId = "f9a18ea2ae15426a8665d4df92feb418";
            string redirectUri = "http://localhost:8888/callback";
            string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\harry\Documents\access files";

            var authURL = new AuthURL(clientId, redirectUri);
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
            db.SaveAuthorizationCode(authorizationCode, DateTime.Now);
        }
    }
}
