using System;
using System.Linq;
using System.Threading.Tasks;

namespace PostTry
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var tokenService = new TokenService();
            string authorizationCode = "YOUR_AUTHORIZATION_CODE"; // Replace with the authorization code you receive after user login
            string accessToken = await tokenService.GetAccessTokenAsync(authorizationCode);

            var getRecommendations = new GetRecommendations();
            var recommendations = await getRecommendations.GetRecommendationsAsync(accessToken, "6IZvVAP7VPPnsGX6bvgkqg,51c94ac31swyDQj9B3Lzs3,0jyikFM0Umv0KlnrOEKtTG,6dBUzqjtbnIa1TwYbyw5CM,3kQfBtkQqgN1fAMfhks8TU");

            Console.WriteLine("Recommended Tracks:");
            foreach (var track in recommendations["tracks"])
            {
                var name = track["name"];
                var artists = string.Join(", ", track["artists"].Select(artist => artist["name"].ToString()));
                Console.WriteLine($"{name} by {artists}");
            }
        }
    }
}
