using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace SpotifyAuthExample
{
    public class AuthURL
    {
        private string _clientId;
        private string _redirectUri;
        private string _verifier;
        private string _challenge;

        public AuthURL(string clientId, string redirectUri)
        {
            _clientId = clientId;
            _redirectUri = redirectUri;
            (_verifier, _challenge) = PKCEUtil.GenerateCodes();
        }

        public void GenerateAuthorizationURL()
        {
            var loginRequest = new LoginRequest(
                new Uri(_redirectUri),
                _clientId,
                LoginRequest.ResponseType.Code
            )
            {
                CodeChallengeMethod = "S256",
                CodeChallenge = _challenge,
                Scope = new[] { Scopes.UserTopRead } // Define scopes
            };
            //taken from https://johnnycrazy.github.io/SpotifyAPI-NET/docs/unit_testing

            var authorizationUri = loginRequest.ToUri();
            Process.Start(new ProcessStartInfo
            {
                FileName = authorizationUri.ToString(),
                UseShellExecute = true
            });
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
        //used  https://johnnycrazy.github.io/SpotifyAPI-NET/docs/unit_testing
    }
}
