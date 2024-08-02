using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostTry
{
    internal class GetUser_SavedTracks
    {
        public async Task GetUserSavedTracks(string token) 
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me/tracks/contains");
            request.Headers.Add("Authorization", "Bearer BQB-KbBpCj-n-CGFZvOg90G8D08MKQFGpaNK60TLxa2E1s_4SxvtiI7jh-Ioo_dp9KKz4OIvGy7_kYkFRLreIF8NkdTS1tsv13yw4CH9xlkagO_8uh0");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());

        }
    }
}
