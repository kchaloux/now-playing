using HtmlAgilityPack;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using NowPlaying.Events;
using NowPlaying.Extensions;
using Timer = System.Timers.Timer;

namespace NowPlaying.Model
{
    /// <summary>
    /// Watches the remote URL for changes to the Now Playing information and raises an event when it changes.
    /// </summary>
    public class NowPlayingWatcher : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="Configuration"/> to use when scraping data from the remote URL.
        /// </summary>
        public Configuration Configuration { get; }

        /// <summary>
        /// Gets the <see cref="Logger"/> being used by this watcher.
        /// </summary>
        public Logger Logger { get; }

        /// <summary>
        /// Gets whether or not the watcher is currently started.
        /// </summary>
        public bool IsRunning { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the Now Playing information is changed.
        /// </summary>
        public event EventHandler<NowPlayingChangedEventArgs> NowPlayingChanged;

        private void OnNowPlayingChanged(NowPlayingInfo newInfo)
        {
            if (_lastNowPlayingInfo != newInfo &&
                _lastNowPlayingInfo?.IsEquivalentTo(newInfo) != true)
            {
                NowPlayingChanged?.Invoke(this, new NowPlayingChangedEventArgs(_lastNowPlayingInfo, newInfo));
                _lastNowPlayingInfo = newInfo;
            }
        }

        #endregion

        #region Fields

        private object _lockObject = new object();
        private CancellationTokenSource _cancellationTokenSource;
        private NowPlayingInfo _lastNowPlayingInfo;
        private readonly Timer _timer;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configuration">The <see cref="Configuration"/> to use when scraping data from the remote URL.</param>
        /// <param name="logger">The <see cref="Logger"/> being used by this watcher.</param>
        public NowPlayingWatcher(Configuration configuration, Logger logger)
        {
            Configuration = configuration;
            Logger = logger;
            _timer = new Timer(configuration.PollingInterval);
            _timer.Elapsed += TimerCallback;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Begin watching the Now Playing info for changes.
        /// </summary>
        public void Start()
        {
            lock (_lockObject)
            {
                if (IsRunning)
                {
                    return;
                }
                _cancellationTokenSource = new CancellationTokenSource();
                _timer.Start();
                IsRunning = true;
            }
        }

        /// <summary>
        /// Stop watching the Now Playing info for changes.
        /// </summary>
        public void Stop()
        {
            lock (_lockObject)
            {
                if (!IsRunning)
                {
                    return;
                }
                _timer.Stop();
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
                IsRunning = false;
            }
        }

        private async void TimerCallback(object sender, ElapsedEventArgs args)
        {
            var cancellationToken = _cancellationTokenSource?.Token ?? CancellationToken.None;
            try
            {
                var remoteUrl = Configuration.SourceUrl.ToString();
                var web = new HtmlWeb();
                var doc = await web.LoadFromWebAsync(remoteUrl, Encoding.UTF8, cancellationToken);

                var infoDiv = doc.DocumentNode.FirstDescendantWithClass("div", "card horizontal");
                if (infoDiv != null)
                {
                    var imgUrl = infoDiv.Descendants("img").FirstOrDefault()?.GetAttributeValue("src", default(string));

                    var content = infoDiv.FirstDescendantWithClass("div", "card-content")?.InnerText;
                    if (content != null)
                    {
                        var lines = content
                            .Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .Take(2)
                            .ToList();

                        var song = lines[0];
                        var artist = lines[1];
                        OnNowPlayingChanged(new NowPlayingInfo(artist, song, imgUrl));
                        return;
                    }
                }
                OnNowPlayingChanged(null);
            }
            catch (Exception ex)
            {
                Logger?.Log(ex);
                OnNowPlayingChanged(null);
            }
        }

        #endregion

        #region IDisposable

        private bool _isDisposed;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        private void Dispose(bool isDisposing)
        {
            if (_isDisposed)
            {
                return;
            }
            if (isDisposing)
            {
                lock (_lockObject)
                {
                    _timer.Stop();
                    _cancellationTokenSource?.Cancel();
                    _cancellationTokenSource?.Dispose();
                    _cancellationTokenSource = null;
                    IsRunning = false;
                }
            }
            _isDisposed = true;
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~NowPlayingWatcher()
        {
            Dispose(false);
        }

        #endregion
    }
}
