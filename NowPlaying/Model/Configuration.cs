using System.Text.Json;
using System.Text.Json.Serialization;

namespace NowPlaying.Model
{
    /// <summary>
    /// Loads and Saves user confirmation information.
    /// </summary>
    public class Configuration
    {
        #region Properties

        /// <summary>
        /// Gets or Sets the URI to load remote data from the YTMDesktop application.
        /// </summary>
        [JsonPropertyName("source-url")]
        public string SourceUrl { get; set; } = "http://localhost:9863/";

        /// <summary>
        /// Gets or Sets the interval at which the <see cref="NowPlayingWatcher"/> polls for changes, in milliseconds.
        /// </summary>
        [JsonPropertyName("polling-interval")]
        public int PollingInterval { get; set; } = 1000;

        /// <summary>
        /// Gets or Sets the path to export the album art to.
        /// </summary>
        [JsonPropertyName("album-art-path")]
        public string AlbumArtPath { get; set; } = "./album.png";

        /// <summary>
        /// Gets or Sets the path to export the song information text to.
        /// </summary>
        [JsonPropertyName("song-info-path")]
        public string SongInfoPath { get; set; } = "./now-playing.txt";

        /// <summary>
        /// Gets or Sets the format string to use when exporting the song data.
        /// </summary>
        /// <remarks>
        /// {song} will be replaced with the name of the current song playing
        /// {artist} will be replaced with the name of the current artist playing
        /// e.g. "{song}" by {artist} could export as "Bohemian Rhapsody" by Queen
        /// </remarks>
        [JsonPropertyName("song-info-format")]
        public string SongInfoFormat { get; set; } = "{song} by {artist}";

        /// <summary>
        /// Gets or Sets the path to write log files to.
        /// </summary>
        [JsonPropertyName("log-file-path")]
        public string LogFilePath { get; set; } = "./log.txt";

        /// <summary>
        /// Gets or Sets whether or not logging is enabled.
        /// </summary>
        [JsonPropertyName("enable-logging")]
        public bool EnableLogging { get; set; } = false;

        #endregion

        #region Methods

        /// <summary>
        /// Deserialize JSON text to a <see cref="Configuration"/> object.
        /// </summary>
        /// <param name="json">The JSON text to deserialize.</param>
        /// <returns>A new <see cref="Configuration"/> object deserialized from the given json configuration.</returns>
        public static Configuration Deserialize(string json)
        {
            return JsonSerializer.Deserialize<Configuration>(json, new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
            });
        }

        /// <summary>
        /// Serialize this <see cref="Configuration"/> object to a JSON string.
        /// </summary>
        /// <returns>A JSON string containing this configuration.</returns>
        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

        #endregion
    }
}
