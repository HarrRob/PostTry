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
            return $"""
                Track Information
                ----------------
                Name: {TrackName}
                Artists: {string.Join(", ", ArtistNames)}
                Album: {AlbumName}
                Genres: {(Genres.Any() ? string.Join(", ", Genres) : "No genres available")}

                Audio Features
                -------------
                Popularity: {PopularityScore}
                Danceability: {DanceabilityScore:0.00}
                Energy: {EnergyScore:0.00}
                Speechiness: {SpeechinessScore:0.00}
                Acousticness: {AcousticnessScore:0.00}
                Instrumentalness: {InstrumentalnessScore:0.00}
                Liveness: {LivenessScore:0.00}
                Valence: {ValenceScore:0.00}

                Technical Details
                ----------------
                Key: {KeyValue}
                Mode: {(ModeValue == 1 ? "Major" : "Minor")}
                Tempo: {TempoValue:0.00} BPM
                Loudness: {LoudnessValue:0.00} dB
                Time Signature: {TimeSignatureValue}/4
                Duration: {TimeSpan.FromMilliseconds(DurationMs):mm\\:ss}
                """;
        }
    }

    public class TrackAnalyzer
    {
        private readonly ISpotifyClient _spotifyClient;
        private const int MaxRetries = 3;
        private const int RetryDelayMs = 1000;

        public TrackAnalyzer(ISpotifyClient client)
        {
            _spotifyClient = client ?? throw new ArgumentNullException(nameof(client));
        }

        private async Task<TracksAudioFeaturesResponse?> GetAudioFeaturesWithRetry(List<string> trackIds)
        {
            // Ensure we have valid track IDs
            var validTrackIds = trackIds.Where(id => !string.IsNullOrEmpty(id)).ToList();

            if (!validTrackIds.Any())
            {
                Console.WriteLine("No valid track IDs provided");
                return null;
            }

            for (int i = 0; i < MaxRetries; i++)
            {
                try
                {
                    // Create request with valid track IDs
                    var request = new TracksAudioFeaturesRequest(validTrackIds);
                    var response = await _spotifyClient.Tracks.GetSeveralAudioFeatures(request);

                    if (response?.AudioFeatures == null)
                    {
                        throw new APIException("No audio features returned from API");
                    }

                    return response;
                }
                catch (APIException ex)
                {
                    if (i == MaxRetries - 1)
                    {
                        Console.WriteLine($"Failed to get audio features after {MaxRetries} attempts: {ex.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"Attempt {i + 1} failed, retrying in {RetryDelayMs * (i + 1)}ms...");
                        await Task.Delay(RetryDelayMs * (i + 1));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error getting audio features: {ex.Message}");
                    return null;
                }
            }
            return null;
        }

        public async Task<List<TrackAnalysisInfo>> ProcessTracksAsync(IEnumerable<FullTrack> tracks)
        {
            if (tracks == null) throw new ArgumentNullException(nameof(tracks));

            var results = new List<TrackAnalysisInfo>();
            var tracksList = tracks.ToList();

            try
            {
                // Get all track IDs
                var trackIds = tracksList.Select(t => t.Id).ToList();

                // Get audio features for all tracks in one batch
                var audioFeatures = await GetAudioFeaturesWithRetry(trackIds);
                var audioFeaturesDict = new Dictionary<string, TrackAudioFeatures>();

                if (audioFeatures?.AudioFeatures != null)
                {
                    // Store the audio features in a dictionary where the key is the track ID
                    audioFeaturesDict = audioFeatures.AudioFeatures
                        .Where(af => af != null)
                        .ToDictionary(af => af.Id);
                }

                foreach (var track in tracksList)
                {
                    try
                    {
                        var trackInfo = new TrackAnalysisInfo
                        {
                            TrackId = track.Id,
                            TrackName = track.Name ?? "Unknown Track",
                            ArtistNames = track.Artists?.Select(a => a.Name ?? "Unknown Artist").ToList() ?? new List<string>(),
                            AlbumName = track.Album?.Name ?? "Unknown Album",
                            PopularityScore = track.Popularity,
                            DurationMs = track.DurationMs
                        };

                        // Add audio features if available
                        if (audioFeaturesDict.TryGetValue(track.Id, out var features))
                        {
                            // Populate the audio features into the trackInfo object
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

                        // Add the track information to the results list
                        results.Add(trackInfo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing track {track.Name}: {ex.Message}");
                    }

                    // Delay to avoid hitting API rate limits
                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during batch processing: {ex.Message}");
            }

            return results;
        }

    }



}
