using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using SpotifyAPI.Web;

namespace SpotifyAuth
{
    public class AuthURL
    {
        private readonly string _clientId;
        private readonly string _redirectUri;
        private readonly string _verifier;

        public AuthURL(string clientId, string redirectUri, string verifier)
        {
            _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
            _redirectUri = redirectUri ?? throw new ArgumentNullException(nameof(redirectUri));
            _verifier = verifier ?? throw new ArgumentNullException(nameof(verifier));
        }

        public string GenerateAuthorizationURL()
        {
            string scopes = string.Join(" ", new[]
            {
                "user-top-read",
                "playlist-modify-public",
                "playlist-modify-private",
                "user-read-private",
                "user-read-email",
                "user-library-read",
                "user-read-currently-playing",
                "user-read-playback-state",
                "playlist-read-private"
            });

            string authorizationUrl = $"https://accounts.spotify.com/authorize" +
                $"?response_type=code" +
                $"&client_id={_clientId}" +
                $"&scope={Uri.EscapeDataString(scopes)}" +
                $"&redirect_uri={Uri.EscapeDataString(_redirectUri)}" +
                $"&show_dialog=true";

            try
            {
                Console.WriteLine("Opening browser to authorize...");
                Process.Start(new ProcessStartInfo
                {
                    FileName = authorizationUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening browser: {ex.Message}");
                Console.WriteLine($"Please manually open this URL: {authorizationUrl}");
            }

            return authorizationUrl;
        }

        public SpotifyClient HandleCallback(string authorizationCode)
        {
            if (string.IsNullOrEmpty(authorizationCode))
            {
                Console.WriteLine("Error: Authorization code is null or empty.");
                return null;
            }

            try
            {
                Console.WriteLine("Exchanging authorization code for access token...");

                using var httpClient = new HttpClient();
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("code", authorizationCode),
                    new KeyValuePair<string, string>("client_id", _clientId),
                    new KeyValuePair<string, string>("client_secret", "0ec970e65f81426baa48cdac1d6780b2"),
                    new KeyValuePair<string, string>("redirect_uri", _redirectUri),
                    new KeyValuePair<string, string>("grant_type", "authorization_code")
                });

                var response = httpClient.PostAsync("https://accounts.spotify.com/api/token", content).Result;

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error: Token swap failed with status code {response.StatusCode}");
                    Console.WriteLine($"Error details: {response.Content.ReadAsStringAsync().Result}");
                    return null;
                }

                var tokenData = JsonSerializer.Deserialize<TokenResponse>(response.Content.ReadAsStringAsync().Result);

                if (tokenData == null || string.IsNullOrEmpty(tokenData.AccessToken))
                {
                    Console.WriteLine("Error: Invalid token response from Spotify.");
                    return null;
                }

                Console.WriteLine($"Access Token: {tokenData.AccessToken}");
                Console.WriteLine($"Refresh Token: {tokenData.RefreshToken}");
                Console.WriteLine($"Expires In: {tokenData.ExpiresIn}");

                var config = SpotifyClientConfig.CreateDefault().WithToken(tokenData.AccessToken);
                return new SpotifyClient(config);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during token exchange: {ex.Message}");
                return null;
            }
        }
    }

    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
