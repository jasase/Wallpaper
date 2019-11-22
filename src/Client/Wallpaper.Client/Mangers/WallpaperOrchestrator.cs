using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Framework.Contracts.Extension;
using Framework.Contracts.IocContainer;
using Framework.Contracts.Services.ThreadManaging;
using Plugin.Application.Wallpaper.Client.Model;
using Plugin.Application.Wallpaper.Client.Monitors;

namespace Plugin.Application.Wallpaper.Client.Mangers
{
    public class WallpaperOrchestrator
    {
        private readonly object _refreshLock;
        private readonly Timer _timer;
        private readonly ILogger _logger;

        private readonly List<LocalWallpaper> _playlist;
        private readonly List<MonitorWallpaperInformation> _monitors;
        private readonly List<LocalWallpaper> _playlistLast;

        private int _nextMonitor;

        public AuthenticationManager AuthenticationManager { get; }
        public WallpaperApiClient ApiClient { get; }
        public LocalWallpaperManager WallpaperManager { get; }
        public MonitorsManager MonitorsManager { get; }
        public PlaylistManager PlaylistManager { get; }

        public IEnumerable<LocalWallpaper> Playlist => _playlist;
        public IEnumerable<LocalWallpaper> PlaylistCurrent => _monitors.OrderBy(x => x.LastChanged).Select(x => x.CurrentWallpaper).Where(x => x != null);
        public IEnumerable<LocalWallpaper> PlaylistLast => _playlistLast;

        public event EventHandler WallpaperChanged;

        public WallpaperOrchestrator(IDependencyResolver dependencyResolver)
        {
            _refreshLock = new object();
            _timer = new Timer(x =>
            {
                _logger.Debug("Cyclic rotate timer executed");
                RotatePictureForward();
            });
            _monitors = new List<MonitorWallpaperInformation>();
            _nextMonitor = 0;

            _playlist = new List<LocalWallpaper>();
            _playlistLast = new List<LocalWallpaper>();

            var threadManager = dependencyResolver.GetInstance<IThreadManager>();
            var logManager = dependencyResolver.GetInstance<ILogManager>();
            var eventService = dependencyResolver.GetInstance<IEventService>();

            _logger = logManager.GetLogger<WallpaperOrchestrator>();

            AuthenticationManager = new AuthenticationManager(logManager);
            MonitorsManager = new MonitorsManager(logManager.GetLogger<MonitorsManager>(), logManager);
            ApiClient = new WallpaperApiClient(logManager.GetLogger<WallpaperApiClient>(), AuthenticationManager);

            WallpaperManager = new LocalWallpaperManager(logManager.GetLogger<LocalWallpaperManager>(), threadManager, eventService, ApiClient);
            PlaylistManager = new PlaylistManager(logManager.GetLogger<PlaylistManager>(), ApiClient);
        }

        public void Start()
        {
            var task = RefreshPlaylist();
            task.ContinueWith(x =>
            {
                for (var i = 0; i < MonitorsManager.MonitorCount; i++)
                {
                    RotatePictureForward();
                }
            });
        }

        public void RotatePictureBackward()
        {
            try
            {
                if (_playlistLast.Any())
                {
                    _logger.Debug("Rotating picutre backward");
                    var nextWallpaper = _playlistLast.Last();
                    _playlistLast.Remove(nextWallpaper);
                    var oldWallpaper = SetWallpaperAndGetOld(nextWallpaper);
                    if (oldWallpaper != null)
                    {
                        _playlist.Insert(0, oldWallpaper);
                    }
                }
                else
                {
                    _logger.Info("Rotating picutre backward not possible because no wallpaper in playlist available");
                }
            }
            finally
            {
                WallpaperChanged?.Invoke(this, EventArgs.Empty);
                SetTimer();
            }
        }

        public void RotatePictureForward()
        {
            try
            {
                if (_playlist.Any())
                {
                    _logger.Debug("Rotating picutre forward");
                    var nextWallpaper = _playlist.First();

                    var checkWallpaper = nextWallpaper;
                    while (!checkWallpaper.HasImage)
                    {
                        _playlist.Remove(checkWallpaper);
                        _playlist.Add(checkWallpaper);
                        checkWallpaper = _playlist.First();

                        if (checkWallpaper == nextWallpaper) return;
                    }

                    _playlist.Remove(nextWallpaper);
                    var oldWallpaper = SetWallpaperAndGetOld(nextWallpaper);
                    if (oldWallpaper != null)
                    {
                        _playlistLast.Add(oldWallpaper);
                    }
                }
                else
                {
                    _logger.Info("Rotating picutre forward not possible because no wallpaper in playlist available");
                }
            }
            finally
            {
                WallpaperChanged?.Invoke(this, EventArgs.Empty);
                SetTimer();
                RefreshPlaylist();
            }
        }

        public void ResetMonitors()
        {
            MonitorsManager.ResetMonitorHandles();
            var counter = 0;
            foreach (var monitor in _monitors)
            {
                if (counter < MonitorsManager.MonitorCount)
                {
                    MonitorsManager.SetImageToMonitor(monitor.CurrentWallpaper.Image, counter);
                }

                counter += 1;
            }
        }

        private LocalWallpaper SetWallpaperAndGetOld(LocalWallpaper nextWallpaper)
        {
            if (_nextMonitor >= MonitorsManager.MonitorCount)
            {
                _nextMonitor = 0;
            }

            if (MonitorsManager.MonitorCount != _monitors.Count)
            {
                while (MonitorsManager.MonitorCount < _monitors.Count)
                {
                    _monitors.RemoveAt(_monitors.Count - 1);
                }

                while (MonitorsManager.MonitorCount > _monitors.Count)
                {
                    _monitors.Add(new MonitorWallpaperInformation());
                }
            }

            var curMonitor = _nextMonitor++;
            MonitorsManager.SetImageToMonitor(nextWallpaper.Image, curMonitor);

            CountView(_monitors[curMonitor]);
            var oldWallpaper = _monitors[curMonitor].CurrentWallpaper;

            _monitors[curMonitor].CurrentWallpaper = nextWallpaper;
            _monitors[curMonitor].LastChanged = DateTime.Now;

            return oldWallpaper;
        }

        private Task RefreshPlaylist()
            => Task.Factory.StartNew(() =>
            {
                lock (_refreshLock)
                {
                    if (_playlist.Count <= MonitorsManager.MonitorCount * 2)
                    {
                        _logger.Info("Refreshing playlist started");
                        foreach (var playlistElement in PlaylistManager.GetNext())
                        {
                            var wallpaper = WallpaperManager.GetById(playlistElement.WallpaperId);
                            if (wallpaper.HasValue)
                            {
                                _playlist.Add(wallpaper.Value);
                            }
                        }
                    }

                    while (_playlistLast.Count > 10)
                    {
                        _playlistLast.RemoveAt(0);
                    }
                }
            }).ContinueWith(x =>
            {
                if (x.IsFaulted)
                {
                    _logger.Error(x.Exception, "Refreshing playlist failed");
                }
            });

        private void CountView(MonitorWallpaperInformation information)
        {
            if (DateTime.Now - information.LastChanged >= TimeSpan.FromSeconds(30) && information.LastChanged != DateTime.MinValue)
            {
                var currentWallpaper = information.CurrentWallpaper;
                Task.Factory.StartNew(() =>
                {
                    WallpaperManager.CountView(currentWallpaper);
                });
            }
        }

        private void SetTimer() => _timer.Change(TimeSpan.FromSeconds(60), TimeSpan.FromMilliseconds(-1));
    }

    public class MonitorWallpaperInformation
    {
        public LocalWallpaper CurrentWallpaper { get; set; }
        public DateTime LastChanged { get; set; }
    }
}
