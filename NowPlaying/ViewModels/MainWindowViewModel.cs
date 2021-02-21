using System;
using System.IO;
using System.Net;
using Prism.Mvvm;
using Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using NowPlaying.Model;
using NowPlaying.Events;


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

        /// <summary>
        /// Interaction request to raise the <see cref="EditConfigurationCommand"/>.
        /// </summary>
        public InteractionRequest<Confirmation> LaunchEditConfigurationCommandRequest { get; }
            = new InteractionRequest<Confirmation>();

        private void OnEditConfigurationCommandExecuted()
        {
            _watcher.Stop();
            var viewModel = new ConfigurationViewModel();
            viewModel.LoadParameters(_configuration);

            var confirmation = new Confirmation()
            {
                Title = "Edit Configuration",
                Content = viewModel,
            };

            LaunchEditConfigurationCommandRequest.Raise(confirmation, conf =>
            {
                if (conf.Confirmed)
                {
                    viewModel.SaveParameters(_configuration);
                    SaveConfiguration(_configuration, ConfigurationPath);
                }
                _watcher.Start();
            });
        }

        #endregion

        #region Fields

        private const string ConfigurationPath = "Resources/config.json";
        private readonly Configuration _configuration;
        private readonly Logger _logger;
        private readonly NowPlayingWatcher _watcher;

        #endregion

        #region Constructors

        public MainWindowViewModel()
        {
            Title = "Now Playing";

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
            _logger?.Log("Now Playing Changed");
            if (e.ImageChanged)
            {
                _logger?.Log($"  Image \"{e.OldInfo?.ImageUrl}\" => \"{e.NewInfo?.ImageUrl}\"");
                UpdateImage(e.NewInfo?.ImageUrl);
            }
            if (e.SongChanged && e.NewInfo?.Song != "")
            {
                _logger?.Log($"  Artist \"{e.OldInfo?.Artist}\" => \"{e.NewInfo?.Artist}\"");
                _logger?.Log($"  Song \"{e.OldInfo?.Song}\" => \"{e.NewInfo?.Song}\"");
                UpdateSongInfo(e.NewInfo?.Artist, e.NewInfo?.Song);
            }
            var artist = e.NewInfo?.Artist ?? "Unknown Artist";
            var song = e.NewInfo?.Song ?? "Unknown Song";
            Title = $"{song} by {artist}";
        }

        #endregion
    }
}
