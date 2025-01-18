using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyAuthExample
{
    public class UserTopTracks
    {
        private readonly SpotifyClient _spotifyClient;

        public UserTopTracks(SpotifyClient spotifyClient)
        {
            _spotifyClient = spotifyClient;
        }

        public async Task<List<FullTrack>> GetTopTracksAsync(int limit)
        {
            try
            {
                var topTracks = await _spotifyClient.Personalization.GetTopTracks(new PersonalizationTopRequest
                {
                    Limit = limit
                });

                return topTracks.Items.ToList();
            }
            catch (APIException apiEx)
            {
                Console.WriteLine($"API Error fetching top tracks: {apiEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching top tracks: {ex.Message}");
                return null;
            }
        }
    }
}
