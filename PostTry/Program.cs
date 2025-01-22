using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using PostTry;  // Ensure this is added to reference MetaData class
using SpotifyAuth;
using SpotifyAPI.Web;

namespace SpotifyAuthExample
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                DatabaseHelper.InitializeDatabase();
                Console.WriteLine("Input first name:");
                string firstName = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(firstName))
                {
                    Console.WriteLine("First name cannot be empty.");
                    return;
                }
                Console.Clear();

                Console.WriteLine("Input last name:");
                string lastName = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(lastName))
                {
                    Console.WriteLine("Last name cannot be empty.");
                    return;
                }
                Console.Clear();

                string clientId = "f9a18ea2ae15426a8665d4df92feb418";
                string redirectUri = "http://localhost:8888/callback";
                var PKCE_verifier = PKCEUtil.GenerateCodeVerifier();
                AuthURL authURL = new AuthURL(clientId, redirectUri, PKCE_verifier);
                var localServer = new LocalServer();
                _ = localServer.StartLocalServer();

                // Generate the URL for the user to authenticate
                string authUrl = authURL.GenerateAuthorizationURL();

                Console.WriteLine("Please open the following URL to authenticate:");
                Console.WriteLine(authUrl);
                Console.WriteLine("Waiting for authorization code...");

                // Loop until the authorization code is received
                string authorizationCode = string.Empty;
                while (string.IsNullOrEmpty(authorizationCode))
                {
                    authorizationCode = localServer.GetAuthorizationCode();
                    if (string.IsNullOrEmpty(authorizationCode))
                    {
                        await Task.Delay(1000); // Wait 1 second before trying again
                    }
                }

                if (string.IsNullOrEmpty(authorizationCode))
                {
                    Console.WriteLine("Authorization code not received.");
                    return;
                }

                // Obtain token data using the authorization code
                var tokenData = authURL.HandleCallback(authorizationCode);

                if (tokenData == null)
                {
                    Console.WriteLine("Failed to get token data.");
                    return;
                }

                // Create SpotifyClient instance using the access token
                var config = SpotifyClientConfig.CreateDefault();
                var spotifyClient = new SpotifyClient(config.WithToken(tokenData.AccessToken)); // Use WithToken method

                // Save user information to the database (uncomment when needed)
                //DatabaseHelper.addUserInfo(firstName, lastName, authorizationCode);

                // Fetch and display the user's top tracks using the spotifyClient
                UserTopTracks userTopTracks = new UserTopTracks(spotifyClient);  // Pass the SpotifyClient here
                var topTracks = await userTopTracks.GetTopTracksAsync(10);

                if (topTracks != null && topTracks.Any())
                {
                    Console.WriteLine("Your top tracks are:");
                    foreach (var track in topTracks)
                    {
                        Console.WriteLine($"{track.Name} by {string.Join(", ", track.Artists.Select(a => a.Name))}");
                    }

                    // Now call TrackAnalyzer to fetch additional metadata for the top tracks
                    // Assuming RefreshAccessToken returns an object with AccessToken property
                    var refreshedToken = authURL.RefreshAccessToken(tokenData.RefreshToken);
                    var newConfig = SpotifyClientConfig.CreateDefault()
                           .WithToken(tokenData.AccessToken);  // Use AccessToken from the refreshed token

                    var newSpotifyClient = new SpotifyClient(newConfig);
                    var analyzer = new TrackAnalyzer(newSpotifyClient);
                    var trackMetadata = await analyzer.ProcessTracksAsync(topTracks);

                    Console.WriteLine("\nTrack Metadata:");
                    foreach (var metadata in trackMetadata)
                    {
                        Console.WriteLine("\n" + metadata.ToString());
                        Console.WriteLine("----------------------");
                    }
                }
                else
                {
                    Console.WriteLine("No top tracks found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
