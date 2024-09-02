using System;
using System.Diagnostics;

namespace SpotifyAuth
{
    public class AuthURL
    {
        public string GenerateAuthorizationURL()
        {
            string clientId = "f9a18ea2ae15426a8665d4df92feb418"; // Your Spotify client ID
            string redirectUri = "https://oauth.pstmn.io/v1/callback"; // Your registered redirect URI
            string scopes = "user-top-read playlist-modify-public playlist-modify-private"; // Adjust scopes as needed

            // Construct the authorization URL
            string authorizationUrl = $"https://accounts.spotify.com/authorize?response_type=code&client_id={clientId}&scope={Uri.EscapeDataString(scopes)}&redirect_uri={Uri.EscapeDataString(redirectUri)}";

            // Open the URL in the default web browser
            Process.Start(new ProcessStartInfo
            {
                FileName = authorizationUrl,
                UseShellExecute = true // Ensures the URL is opened in the default web browser
            });

            return authorizationUrl;
        }
    }
}
