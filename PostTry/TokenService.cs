using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PostTry
{
    internal class TokenService
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<string> GetAccessTokenAsync(string authorizationCode)
        {
            // Replace with your Base64-encoded client ID and secret
            string clientCredentials = "your_base64_encoded_client_id_and_secret";
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
            {
                Headers = { Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", clientCredentials) },
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", authorizationCode),
                    new KeyValuePair<string, string>("redirect_uri", "your_redirect_uri") // Must match the redirect URI used during authorization
                })
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            return json["access_token"]?.ToString();
        }
    }
}
