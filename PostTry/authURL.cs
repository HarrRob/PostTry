using SpotifyAPI.Web;
using System.Diagnostics;

namespace SpotifyAuth
{
    public class AuthURL
    {
        private readonly string _clientId;
        private readonly string _redirectUri;
        private readonly string _verifier;

        public AuthURL(string clientId, string redirectUri, string verifier)
        {
            _clientId = clientId;
            _redirectUri = redirectUri;
            _verifier = verifier;
        }

        public string GenerateAuthorizationURL()
        {
            string scopes = "user-top-read playlist-modify-public playlist-modify-private"; // Adjust scopes as needed

            // Construct the authorization URL
            string authorizationUrl = $"https://accounts.spotify.com/authorize?response_type=code&client_id={_clientId}&scope={Uri.EscapeDataString(scopes)}&redirect_uri={Uri.EscapeDataString(_redirectUri)}";

            // Open the URL in the default web browser
            Process.Start(new ProcessStartInfo
            {
                FileName = authorizationUrl,
                UseShellExecute = true // Ensures the URL is opened in the default web browser
            });

            return authorizationUrl;
        }

        public async Task<SpotifyClient> HandleCallback(string authorizationCode)
        {
            var oauthClient = new OAuthClient();
            var tokenRequest = new PKCETokenRequest(
                _clientId,
                authorizationCode,
                new Uri(_redirectUri),
                _verifier
            );

            var tokenResponse = await oauthClient.RequestToken(tokenRequest);
            Console.WriteLine("Access Token: " + tokenResponse.AccessToken);
            Console.WriteLine("Refresh Token: " + tokenResponse.RefreshToken);

            return new SpotifyClient(tokenResponse.AccessToken);
        }
    }
}
