using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class GetReccomendations
{
    private static readonly HttpClient client = new HttpClient();

    public async Task<JObject> GetRecommendationsAsync(string accessToken, string seedArtists = null, string seedTracks = null, int limit = 10)
    {
        try
        {
            var requestUri = "https://api.spotify.com/v1/recommendations";

            // Build query parameters
            var queryParameters = $"?limit={limit}";

            if (!string.IsNullOrEmpty(seedArtists))
                queryParameters += $"&seed_artists={seedArtists}";

            if (!string.IsNullOrEmpty(seedTracks))
                queryParameters += $"&seed_tracks={seedTracks}";

            requestUri += queryParameters;

            // Prepare the request
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            // Send the request
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Read the response body
            var responseBody = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseBody);

            return json;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
            throw; // Re-throw the exception to let the calling code handle it
        }
        catch (Exception e)
        {
            Console.WriteLine($"General error: {e.Message}");
            throw;
        }
    }
}
