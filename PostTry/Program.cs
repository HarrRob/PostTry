using System;
using System.Threading.Tasks;

namespace PostTry
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            TokenService tokenService = new TokenService();
            string token = await tokenService.GetAccessTokenAsync();
            var getReccomendations = new GetReccomendations();
            var recommendations = await getReccomendations.GetRecommendationsAsync(token, seedArtists: "artist_id1,artist_id2", seedTracks: "track_id1,track_id2");

            Console.WriteLine("Recommendations:");
            Console.WriteLine(recommendations.ToString());
        }
    }
}
