using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PostTry
{
    internal class TokenService
    {
        public async Task<string> GetAccessTokenAsync()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");

            request.Headers.Add("Authorization", "Basic ZjlhMThlYTJhZTE1NDI2YTg2NjVkNGRmOTJmZWI0MTg6MGVjOTcwZTY1ZjgxNDI2YmFhNDhjZGFjMWQ2NzgwYjI=");
            request.Headers.Add("Cookie", "__Host-device_id=AQC0HksgOqqtxnMGLBTi-z7OoDCSPfQQCrCUti7ddhM4WVpvWRiX929YdG07df-xlazsaTJkrKWm3fFenQXvIDUJIpSfpisKGSk");

            var collection = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            };

            var content = new FormUrlEncodedContent(collection);
            request.Content = content;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);

            // Parse the JSON response to extract the access token
            var json = JObject.Parse(responseContent);
            var accessToken = json["access_token"]?.ToString();

            Console.WriteLine("Access Token: " + accessToken);

            return accessToken;
        }
    }
}
