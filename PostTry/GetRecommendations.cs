using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PostTry
{
    internal class GetRecommendations
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<JObject> GetRecommendationsAsync(string accessToken, string seedTracks)
        {
            var requestUri = $"https://api.spotify.com/v1/recommendations?limit=5&seed_tracks={seedTracks}";

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            return JObject.Parse(responseBody);
        }
    }
}
