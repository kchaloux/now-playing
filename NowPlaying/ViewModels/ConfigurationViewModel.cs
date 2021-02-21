using Prism.Mvvm;
using NowPlaying.Model;

namespace NowPlaying.ViewModels
{
    /// <summary>
    /// View model for editing a <see cref="Configuration"/>'s parameters.
    /// </summary>
    public class ConfigurationViewModel : BindableBase
    {
        #region Properties
        /// <summary>
        /// Gets or Sets the URI to load remote data from the YTMDesktop application.
        /// </summary>
        public string SourceUrl
        {
            get => _sourceUrl;
            set => SetProperty(ref _sourceUrl, value);
        }
        private string _sourceUrl;

        /// <summary>
        /// Gets or Sets the interval at which the <see cref="NowPlayingWatcher"/> polls for changes, in milliseconds.
        /// </summary>
        public int PollingInterval
        {
            get => _pollingInterval;
            set => SetProperty(ref _pollingInterval, value);
        }
        private int _pollingInterval;

        /// <summary>
        /// Gets or Sets the path to export the album art to.
        /// </summary>
        public string AlbumArtPath
        {
            get => _albumArtPath;
            set => SetProperty(ref _albumArtPath, value);
        }
        private string _albumArtPath;

        /// <summary>
        /// Gets or Sets the path to export the song information text to.
        /// </summary>
        public string SongInfoPath
        {
            get => _songInfoPath;
            set => SetProperty(ref _songInfoPath, value);
        }
        private string _songInfoPath;

        /// <summary>
        /// Gets or Sets the format string to use when exporting the song data.
        /// </summary>
        /// <remarks>
        /// {song} will be replaced with the name of the current song playing
        /// {artist} will be replaced with the name of the current artist playing
        /// e.g. "{song}" by {artist} could export as "Bohemian Rhapsody" by Queen
        /// </remarks>
        public string SongInfoFormat
        {
            get => _songInfoFormat;
            set => SetProperty(ref _songInfoFormat, value);
        }
        private string _songInfoFormat;

        /// <summary>
        /// Gets or Sets the path to write log files to.
        /// </summary>
        public string LogFilePath
        {
            get => _logFilePath;
            set => SetProperty(ref _logFilePath, value);
        }
        private string _logFilePath;

        /// <summary>
        /// Gets or Sets whether or not logging is enabled.
        /// </summary>
        public bool EnableLogging
        {
            get => _enableLogging;
            set => SetProperty(ref _enableLogging, value);
        }
        private bool _enableLogging;

        #endregion

        #region Methods

        /// <summary>
        /// Load the parameters of a <see cref="Configuration"/> to edit.
        /// </summary>
        /// <param name="config">The <see cref="Configuration"/> to load the parameters from.</param>
        public void LoadParameters(Configuration config)
        {
            SourceUrl = config.SourceUrl;
            PollingInterval = config.PollingInterval;
            AlbumArtPath = config.AlbumArtPath;
            SongInfoPath = config.SongInfoPath;
            SongInfoFormat = config.SongInfoFormat;
            LogFilePath = config.LogFilePath;
            EnableLogging = config.EnableLogging;
        }

        /// <summary>
        /// Save the current parameters from the view model to the given <see cref="Configuration"/>.
        /// </summary>
        /// <param name="config">The <see cref="Configuration"/> to save the view model's parameters to.</param>
        public void SaveParameters(Configuration config)
        {
            config.SourceUrl = SourceUrl;
            config.PollingInterval = PollingInterval;
            config.AlbumArtPath = AlbumArtPath;
            config.SongInfoPath = SongInfoPath;
            config.SongInfoFormat = SongInfoFormat;
            config.LogFilePath = LogFilePath;
            config.EnableLogging = EnableLogging;
        }

        #endregion
    }
}
