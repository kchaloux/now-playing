using System;
using System.IO;
using Microsoft.Win32;
using Prism.Mvvm;
using NowPlaying.Model;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace NowPlaying.ViewModels
{
    /// <summary>
    /// View model for editing a <see cref="Configuration"/>'s parameters.
    /// </summary>
    public class ConfigurationDialogViewModel : BindableBase, IDialogAware
    {
        #region Properties

        /// <summary>
        /// The title of the dialog that will show in the window title bar.
        /// </summary>
        public string Title { get; } = "Configure Settings";

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
        /// Gets or Sets the number of pixels to save the album art height and width to.
        /// </summary>
        public int AlbumArtSize
        {
            get => _albumArtSize;
            set => SetProperty(ref _albumArtSize, value);
        }
        private int _albumArtSize;

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

        #region Commands

        #region OkCommand

        /// <summary>
        /// Close the dialog and return and accept the changes.
        /// </summary>
        public DelegateCommand OkCommand { get; }

        private void OnOkCommandExecuted()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters
            {
                { nameof(Configuration), CreateConfiguration() },
            }));
        }

        #endregion

        #region CancelCommand

        /// <summary>
        /// Close the dialog and discard the changes.
        /// </summary>
        public DelegateCommand CancelCommand { get; }

        private void OnCancelCommandExecuted()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        #endregion

        #region BrowseAlbumArtCommand

        public DelegateCommand BrowseAlbumArtCommand { get; }

        private void OnBrowseAlbumArtCommandExecuted()
        {
            AlbumArtPath = BrowseFile(AlbumArtPath, "JPEG Images (*.jpg, *.jpeg)| *.jpg; *.jpeg");
        }

        #endregion

        #region BrowseSongInfoCommand

        public DelegateCommand BrowseSongInfoCommand { get; }

        private void OnBrowseSongInfoCommandExecuted()
        {
            SongInfoPath = BrowseFile(SongInfoPath, "Text Files (*.txt) | *.txt");
        }

        #endregion

        #region BrowseLogFileCommand

        public DelegateCommand BrowseLogFileCommand { get; }

        private void OnBrowseLogFileCommandExecuted()
        {
            LogFilePath = BrowseFile(LogFilePath, "Text Files (*.txt)|*.txt");
        }

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Instructs the <see cref="T:Prism.Services.Dialogs.IDialogWindow" /> to close the dialog.
        /// </summary>
        public event Action<IDialogResult> RequestClose;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public ConfigurationDialogViewModel()
        {
            OkCommand = new DelegateCommand(OnOkCommandExecuted);
            CancelCommand = new DelegateCommand(OnCancelCommandExecuted);
            BrowseAlbumArtCommand = new DelegateCommand(OnBrowseAlbumArtCommandExecuted);
            BrowseSongInfoCommand = new DelegateCommand(OnBrowseSongInfoCommandExecuted);
            BrowseLogFileCommand = new DelegateCommand(OnBrowseLogFileCommandExecuted);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines if the dialog can be closed.
        /// </summary>
        /// <returns>If <c>true</c> the dialog can be closed. If <c>false</c> the dialog will not close.</returns>
        public bool CanCloseDialog()
        {
            return true;
        }

        /// <summary>
        /// Called when the dialog is closed.
        /// </summary>
        public void OnDialogClosed()
        {
        }

        /// <summary>
        /// Called when the dialog is opened.
        /// </summary>
        /// <param name="parameters">The parameters passed to the dialog.</param>
        public void OnDialogOpened(IDialogParameters parameters)
        {
            LoadConfiguration(parameters.GetValue<Configuration>("Configuration"));
        }

        private void LoadConfiguration(Configuration configuration)
        {
            SourceUrl = configuration.SourceUrl;
            PollingInterval = configuration.PollingInterval;
            AlbumArtSize = configuration.AlbumArtSize;
            AlbumArtPath = configuration.AlbumArtPath;
            SongInfoPath = configuration.SongInfoPath;
            SongInfoFormat = configuration.SongInfoFormat;
            LogFilePath = configuration.LogFilePath;
            EnableLogging = configuration.EnableLogging;
        }

        private Configuration CreateConfiguration()
        {
            return new Configuration
            {
                SourceUrl = SourceUrl,
                PollingInterval = PollingInterval,
                AlbumArtSize = AlbumArtSize,
                AlbumArtPath = AlbumArtPath,
                SongInfoPath = SongInfoPath,
                SongInfoFormat = SongInfoFormat,
                LogFilePath = LogFilePath,
                EnableLogging = EnableLogging,
            };
        }

        private static string BrowseFile(string filePath, string filter)
        {
            var initialDirectory = Path.GetDirectoryName(filePath);
            if (string.IsNullOrWhiteSpace(initialDirectory))
            {
                initialDirectory = Directory.GetCurrentDirectory();
            }
            var fileName = Path.GetFileName(filePath);

            var saveFileDialog = new SaveFileDialog
            {
                Filter = filter,
                FileName = fileName,
                InitialDirectory = initialDirectory,
            };
            return saveFileDialog.ShowDialog() == true
                ? saveFileDialog.FileName
                : filePath;
        }

        #endregion
    }
}
