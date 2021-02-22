using System;
using System.IO;
using System.Net;
using Prism.Mvvm;
using Prism.Commands;
using NowPlaying.Model;
using NowPlaying.Events;
using Prism.Services.Dialogs;


namespace NowPlaying.ViewModels
{
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

        #endregion

        #region Commands

        /// <summary>
        /// Shut the program down.
        /// </summary>
        public DelegateCommand ShutdownCommand { get; }

        private void OnShutdownCommandExecuted()
        {
            _watcher.NowPlayingChanged -= WatcherOnNowPlayingChanged;
            _watcher.Stop();
            _watcher.Dispose();
            _logger?.Log("Shutting Down");
            SaveConfiguration(_configuration, ConfigurationPath);
            Environment.Exit(0);
        }

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

        #region Fields

        private const string ConfigurationPath = "Resources/config.json";
        private readonly Configuration _configuration;
        private readonly Logger _logger;
        private readonly NowPlayingWatcher _watcher;
        private readonly IDialogService _dialogService;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindowViewModel(IDialogService dialogService)
        {
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

        private void UpdateImage(string imageUrl)
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
                    client.DownloadFile(new Uri(imageUrl), _configuration.AlbumArtPath);
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
                var songInfo = _configuration.SongInfoFormat.Replace("{song}", song).Replace("{artist}", artist);
                File.AppendAllText(_configuration.SongInfoPath, songInfo);
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
            if (e.ImageChanged)
            {
                UpdateImage(e.NewInfo?.ImageUrl);
            }
            if (e.SongChanged && e.NewInfo?.Song != "")
            {
                UpdateSongInfo(e.NewInfo?.Artist, e.NewInfo?.Song);
                var artist = e.NewInfo?.Artist ?? "Unknown Artist";
                var song = e.NewInfo?.Song ?? "Unknown Song";
                Title = $"Now playing \"{song}\" by {artist}";
            }
            else
            {
                Title = "Not song playing";
            }
            _logger?.Log(Title);
        }

        #endregion
    }
}
