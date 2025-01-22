using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using Spotifly.Models; // Ensure this is correct if necessary
using ChartJSCore.Models;

namespace PostTry
{
    public class TrackAnalyzer
    {
        private readonly SpotifyClient _spotifyClient;

        public TrackAnalyzer(SpotifyClient client)
        {
            _spotifyClient = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<List<TrackAnalysisInfo>> GetTrackMetadataAsync(IEnumerable<FullTrack> tracks)
        {
            if (tracks == null) throw new ArgumentNullException(nameof(tracks));

            var results = new List<TrackAnalysisInfo>();

            try
            {
                // Get track IDs and fetch audio features in one go
                var trackIds = tracks.Select(t => t.Id).ToList();
                var audioFeaturesResponse = await _spotifyClient.Tracks.GetSeveralAudioFeatures(new TracksAudioFeaturesRequest(trackIds));

                // Simplify track metadata processing
                foreach (var track in tracks)
                {
                    var trackInfo = new TrackAnalysisInfo
                    {
                        TrackId = track.Id,
                        TrackName = track.Name,
                        ArtistNames = track.Artists?.Select(a => a.Name).ToList(),
                        AlbumName = track.Album?.Name,
                        PopularityScore = track.Popularity,
                        DurationMs = track.DurationMs
                    };

                    var features = audioFeaturesResponse.AudioFeatures.FirstOrDefault(f => f.Id == track.Id);
                    if (features != null)
                    {
                        trackInfo.DanceabilityScore = features.Danceability;
                        trackInfo.EnergyScore = features.Energy;
                        trackInfo.KeyValue = features.Key;
                        trackInfo.LoudnessValue = features.Loudness;
                        trackInfo.ModeValue = features.Mode;
                        trackInfo.SpeechinessScore = features.Speechiness;
                        trackInfo.AcousticnessScore = features.Acousticness;
                        trackInfo.InstrumentalnessScore = features.Instrumentalness;
                        trackInfo.LivenessScore = features.Liveness;
                        trackInfo.ValenceScore = features.Valence;
                        trackInfo.TempoValue = features.Tempo;
                        trackInfo.TimeSignatureValue = features.TimeSignature;
                    }

                    results.Add(trackInfo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching track metadata: {ex.Message}");
            }

            return results;
        }
    }
}
namespace Spotifly.Models
{
    public class TrackAnalysisInfo
    {
        public string TrackId { get; set; }
        public string TrackName { get; set; }
        public List<string> ArtistNames { get; set; }
        public string AlbumName { get; set; }
        public int PopularityScore { get; set; }
        public long DurationMs { get; set; }

        // Audio features
        public float DanceabilityScore { get; set; }
        public float EnergyScore { get; set; }
        public int KeyValue { get; set; }
        public float LoudnessValue { get; set; }
        public int ModeValue { get; set; }
        public float SpeechinessScore { get; set; }
        public float AcousticnessScore { get; set; }
        public float InstrumentalnessScore { get; set; }
        public float LivenessScore { get; set; }
        public float ValenceScore { get; set; }
        public float TempoValue { get; set; }
        public int TimeSignatureValue { get; set; }
    }
}

