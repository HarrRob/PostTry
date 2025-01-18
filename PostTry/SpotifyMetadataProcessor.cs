using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostTry
{
    public class TrackAnalysisInfo
    {
        public TrackAnalysisInfo()
        {
            ArtistNames = new List<string>();
            Genres = new List<string>();
        }

        public string TrackId { get; set; } = string.Empty;
        public string TrackName { get; set; } = string.Empty;
        public List<string> ArtistNames { get; set; }
        public string AlbumName { get; set; } = string.Empty;
        public List<string> Genres { get; set; }
        public int PopularityScore { get; set; }
        public double DanceabilityScore { get; set; }
        public double EnergyScore { get; set; }
        public int KeyValue { get; set; }
        public double LoudnessValue { get; set; }
        public int ModeValue { get; set; }
        public double SpeechinessScore { get; set; }
        public double AcousticnessScore { get; set; }
        public double InstrumentalnessScore { get; set; }
        public double LivenessScore { get; set; }
        public double ValenceScore { get; set; }
        public double TempoValue { get; set; }
        public int DurationMs { get; set; }
        public int TimeSignatureValue { get; set; }

        public override string ToString()
        {
            try
            {
                return $"Track: {TrackName}\n" +
                       $"Artists: {string.Join(", ", ArtistNames ?? new List<string>())}\n" +
                       $"Album: {AlbumName}\n" +
                       $"Genres: {string.Join(", ", Genres ?? new List<string>())}\n" +
                       $"Popularity: {PopularityScore}\n" +
                       $"Danceability: {DanceabilityScore:F2}\n" +
                       $"Energy: {EnergyScore:F2}\n" +
                       $"Key: {KeyValue}\n" +
                       $"Tempo: {TempoValue:F2} BPM\n" +
                       $"Duration: {TimeSpan.FromMilliseconds(DurationMs):mm\\:ss}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error formatting track information: {ex.Message}");
                return "Error formatting track information";
            }
        }
    }

    public class TrackAnalyzer
    {
        private readonly ISpotifyClient spotifyClient;

        public TrackAnalyzer(ISpotifyClient client)
        {
            spotifyClient = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<List<TrackAnalysisInfo>> ProcessTracksAsync(IEnumerable<FullTrack> tracks)
        {
            if (tracks == null) throw new ArgumentNullException(nameof(tracks));

            var results = new List<TrackAnalysisInfo>();
            var tracksList = tracks.ToList();

            try
            {
                // Fetch audio features for all tracks in a single API call
                var trackIds = tracksList.Select(t => t.Id).ToList();
                // Create a TracksAudioFeaturesRequest with the list of track IDs
                var audioFeaturesRequest = new TracksAudioFeaturesRequest(trackIds);

                // Fetch audio features using the correct request object
                var audioFeaturesResponse = await spotifyClient.Tracks.GetSeveralAudioFeatures(audioFeaturesRequest);


                if (audioFeaturesResponse?.AudioFeatures == null)
                {
                    throw new Exception("Failed to fetch audio features from Spotify API.");
                }

                // Process each track alongside its corresponding audio features
                for (int i = 0; i < tracksList.Count; i++)
                {
                    var track = tracksList[i];
                    var features = audioFeaturesResponse.AudioFeatures.ElementAtOrDefault(i);

                    if (track == null || features == null) continue;

                    var trackInfo = new TrackAnalysisInfo
                    {
                        TrackId = track.Id ?? string.Empty,
                        TrackName = track.Name ?? string.Empty,
                        ArtistNames = track.Artists?.Select(a => a?.Name ?? "Unknown Artist").ToList() ?? new List<string>(),
                        AlbumName = track.Album?.Name ?? string.Empty,
                        Genres = new List<string>(), // To avoid extra API calls, genres are left empty for now
                        PopularityScore = track.Popularity,

                        // Audio feature properties
                        DanceabilityScore = features.Danceability,
                        EnergyScore = features.Energy,
                        KeyValue = features.Key,
                        LoudnessValue = features.Loudness,
                        ModeValue = features.Mode,
                        SpeechinessScore = features.Speechiness,
                        AcousticnessScore = features.Acousticness,
                        InstrumentalnessScore = features.Instrumentalness,
                        LivenessScore = features.Liveness,
                        ValenceScore = features.Valence,
                        TempoValue = features.Tempo,
                        DurationMs = features.DurationMs,
                        TimeSignatureValue = features.TimeSignature
                    };

                    results.Add(trackInfo);
                }
            }
            catch (APIException ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }

            return results;
        }
    }
}
