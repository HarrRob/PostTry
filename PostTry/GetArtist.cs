using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PostTry
{
    internal class GetArtist
    {
        public async Task GetArtistInfoAsync(string token)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/artists/0TnOYISbd1XYRBk9myaseg");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
        }
    }
}
