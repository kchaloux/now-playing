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
        public string SourceUrl { get; set; }

        /// <summary>
        /// Gets or Sets the interval at which the <see cref="NowPlayingWatcher"/> polls for changes, in milliseconds.
        /// </summary>
        [JsonPropertyName("polling-interval")]
        public int PollingInterval { get; set; }

        /// <summary>
        /// Gets or Sets the path to export the album art to.
        /// </summary>
        [JsonPropertyName("album-art-path")]
        public string AlbumArtPath { get; set; }

        /// <summary>
        /// Gets or Sets the path to export the song information text to.
        /// </summary>
        [JsonPropertyName("song-info-path")]
        public string SongInfoPath { get; set; }

        /// <summary>
        /// Gets or Sets the format string to use when exporting the song data.
        /// </summary>
        /// <remarks>
        /// {song} will be replaced with the name of the current song playing
        /// {artist} will be replaced with the name of the current artist playing
        /// e.g. "{song}" by {artist} could export as "Bohemian Rhapsody" by Queen
        /// </remarks>
        [JsonPropertyName("song-info-format")]
        public string SongInfoFormat { get; set; }

        /// <summary>
        /// Gets or Sets the path to write log files to.
        /// </summary>
        [JsonPropertyName("log-file-path")]
        public string LogFilePath { get; set; }

        /// <summary>
        /// Gets or Sets whether or not logging is enabled.
        /// </summary>
        [JsonPropertyName("enable-logging")]
        public bool EnableLogging { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public Configuration()
        {
            SourceUrl = "http://localhost:9863/";
            PollingInterval = 1000;
            AlbumArtPath = "./album.png";
            SongInfoPath = "./now-playing.txt";
            SongInfoFormat = "{song} by {artist}";
            LogFilePath = "./log.txt";
            EnableLogging = false;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="configuration">The <see cref="Configuration"/> to copy.</param>
        public Configuration(Configuration configuration)
        {
            CopyParameters(configuration);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a clone of this <see cref="Configuration"/>.
        /// </summary>
        /// <returns>A new <see cref="Configuration"/> with the same parameters as this one.</returns>
        public Configuration Clone()
        {
            var configuration = new Configuration();
            configuration.CopyParameters(this);
            return configuration;
        }

        /// <summary>
        /// Copy the parameters of another <see cref="Configuration"/> into this one.
        /// </summary>
        /// <param name="configuration">The other <see cref="Configuration"/> to copy the parameters of.</param>
        public void CopyParameters(Configuration configuration)
        {
            SourceUrl = configuration.SourceUrl;
            PollingInterval = configuration.PollingInterval;
            AlbumArtPath = configuration.AlbumArtPath;
            SongInfoPath = configuration.SongInfoPath;
            SongInfoFormat = configuration.SongInfoFormat;
            LogFilePath = configuration.LogFilePath;
            EnableLogging = configuration.EnableLogging;
        }

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

        /// <summary>
        /// Check whether the values of this <see cref="Configuration"/> are equal to another.
        /// </summary>
        /// <param name="configuration">The other <see cref="Configuration"/> to compare.</param>
        /// <returns>True if the given <see cref="Configuration"/> as the same values as this one.</returns>
        public bool IsEquivalentTo(Configuration configuration)
        {
            return SourceUrl == configuration.SourceUrl
                && PollingInterval == configuration.PollingInterval
                && AlbumArtPath == configuration.AlbumArtPath
                && SongInfoPath == configuration.SongInfoPath
                && SongInfoFormat == configuration.SongInfoFormat
                && LogFilePath == configuration.LogFilePath
                && EnableLogging == configuration.EnableLogging;
        }

        #endregion
    }
}
