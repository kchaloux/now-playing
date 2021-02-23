using System;
using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using NowPlaying.Model;
using NowPlaying.Events;
using Prism.Services.Dialogs;

namespace NowPlaying.ViewModels
{
    /// <summary>
    /// View model for the main window / tray icon.
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        #region Properties

        /// <summary>
        /// Gets or Sets the title to display for both the window and the tray bar icon popup.
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        private string _title;

        /// <summary>
        /// Gets or Sets whether the tray icon should be displayed.
        /// </summary>
        public bool IsTrayIconVisible
        {
            get => _isTrayIconVisible;
            set => SetProperty(ref _isTrayIconVisible, value);
        }
        private bool _isTrayIconVisible = true;

        /// <summary>
        /// Gets or Sets whether or not the context menu is open.
        /// </summary>
        public bool IsContextMenuOpen
        {
            get => _isContextMenuOpen;
            set => SetProperty(ref _isContextMenuOpen, value);
        }
        private bool _isContextMenuOpen;

        #endregion

        #region Commands

        #region Shutdown Command

        /// <summary>
        /// Shut the program down.
        /// </summary>
        public DelegateCommand ShutdownCommand { get; }

        private async void OnShutdownCommandExecuted()
        {
            _logger?.Log("Shutting Down");
            IsTrayIconVisible = false;
            IsContextMenuOpen = false;
            _watcher.NowPlayingChanged -= WatcherOnNowPlayingChanged;
            _watcher.Stop();
            _watcher.Dispose();
            SaveConfiguration(_configuration, ConfigurationPath);
            UpdateSongInfo(null, null);
            UpdateAlbumArt(null);
            await Task.Delay(100); // Allow the context menu time for the close animation to complete
            Environment.Exit(0);
        }

        #endregion

        #region EditConfiguration Command

        /// <summary>
        /// Edit the configuration file.
        /// </summary>
        public DelegateCommand EditConfigurationCommand { get; }

        private void OnEditConfigurationCommandExecuted()
        {
            _watcher.Stop();
            _dialogService.ShowDialog("ConfigurationDialog", new DialogParameters
            {
                { "Configuration", _configuration },
            }, EditConfigurationCallback);
        }

        private void EditConfigurationCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result == ButtonResult.OK)
            {
                var newConfiguration = dialogResult.Parameters.GetValue<Configuration>("Configuration");
                _configuration.CopyParameters(newConfiguration);
            }
            _watcher.Start();
        }

        #endregion

        #endregion

        #region Fields

        private readonly object _lockObject = new object();
        private const string ConfigurationPath = "Resources/config.json";
        private readonly Configuration _configuration;
        private readonly Logger _logger;
        private readonly NowPlayingWatcher _watcher;
        private readonly IDialogService _dialogService;
        private NowPlayingInfo _currentSong;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindowViewModel(IDialogService dialogService)
        {
            Title = "No song playing";

            _dialogService = dialogService;

            ShutdownCommand = new DelegateCommand(OnShutdownCommandExecuted);
            EditConfigurationCommand = new DelegateCommand(OnEditConfigurationCommandExecuted);

            _configuration = LoadConfiguration(ConfigurationPath);
            _logger = new Logger(_configuration);
            _watcher = new NowPlayingWatcher(_configuration, _logger);
            _watcher.NowPlayingChanged += WatcherOnNowPlayingChanged;
            _watcher.Start();
        }

        #endregion

        #region Methods

        private Configuration LoadConfiguration(string path)
        {
            try
            {
                return Configuration.Deserialize(File.ReadAllText(path));
            }
            catch (Exception ex)
            {
                _logger?.Log(ex);
                throw;
            }
        }

        private void SaveConfiguration(Configuration configuration, string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                File.AppendAllText(path, configuration.Serialize());
            }
            catch (Exception ex)
            {
                _logger?.Log(ex);
                throw;
            }
        }

        private void UpdateAlbumArt(string imageUrl)
        {
            try
            {
                if (imageUrl == null)
                {
                    File.Delete(_configuration.AlbumArtPath);
                }
                else
                {
                    using var client = new WebClient();
                    var imageData = client.DownloadData(new Uri(imageUrl));

                    var size = _configuration.AlbumArtSize;
                    using var ms = new MemoryStream(imageData);
                    using var image = Image.FromStream(ms);
                    using var bmp = new Bitmap(image, size, size);
                    bmp.Save(_configuration.AlbumArtPath, ImageFormat.Jpeg);
                }
            }
            catch (Exception ex)
            {
                _logger?.Log(ex);
            }
        }

        private void UpdateSongInfo(string artist, string song)
        {
            try
            {
                if (File.Exists(_configuration.SongInfoPath))
                {
                    File.Delete(_configuration.SongInfoPath);
                }
                if (!string.IsNullOrWhiteSpace(artist) && !string.IsNullOrWhiteSpace(song))
                {
                    var songInfo = _configuration.SongInfoFormat.Replace("{song}", song).Replace("{artist}", artist);
                    File.AppendAllText(_configuration.SongInfoPath, songInfo);
                }
            }
            catch (Exception ex)
            {
                _logger?.Log(ex);
            }
        }

        #endregion

        #region Event Handlers

        private void WatcherOnNowPlayingChanged(object sender, NowPlayingChangedEventArgs e)
        {
            lock (_lockObject)
            {
                if (_currentSong?.IsEquivalentTo(e.NewInfo) == true)
                {
                    return;
                }

                if (!string.IsNullOrWhiteSpace(e.NewInfo?.Artist) && !string.IsNullOrWhiteSpace(e.NewInfo?.Song))
                {
                    if (_currentSong?.ImageUrl != e.NewInfo?.ImageUrl)
                    {
                        UpdateAlbumArt(e.NewInfo?.ImageUrl);
                    }
                    UpdateSongInfo(e.NewInfo?.Artist, e.NewInfo?.Song);
                    var artist = e.NewInfo?.Artist ?? "Unknown Artist";
                    var song = e.NewInfo?.Song ?? "Unknown Song";
                    Title = $"Now playing \"{song}\" by {artist}";
                    _currentSong = e.NewInfo;
                }
                else
                {
                    Title = "No song playing";
                    UpdateSongInfo(null, null);
                    UpdateAlbumArt(null);
                    _currentSong = null;
                }
                _logger?.Log(Title);
            }
        }

        #endregion
    }
}
