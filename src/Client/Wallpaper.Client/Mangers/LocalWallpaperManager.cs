using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Framework.Core.Helper;
using Framework.Abstraction.Extension;
using Framework.Abstraction.Helper;
using Framework.Abstraction.Messages;
using Framework.Abstraction.Services.ThreadManaging;
using Plugin.Application.Wallpaper.Client.Model;
using Web.Commands.Wallpaper;

namespace Plugin.Application.Wallpaper.Client.Mangers
{
    public class LocalWallpaperManager : IMessageReceiver<SystemIsShutingDownMessage>
    {
        private readonly DirectoryInfo _cacheDirectory;
        private readonly Dictionary<Guid, LocalWallpaper> _wallpaper;
        private readonly ILogger _logger;
        private readonly WallpaperApiClient _wallpaperApiClient;
        private readonly BlockingCollection<LocalWallpaper> _downloaderQueue;
        private readonly WallpaperDownloader _downloader;

        public LocalWallpaperManager(ILogger logger,
                                     IThreadManager threadManager,
                                     IEventService eventService,
                                     WallpaperApiClient wallpaperApiClient)
        {
            _wallpaper = new Dictionary<Guid, LocalWallpaper>();
            _cacheDirectory = new DirectoryInfo(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                 "Wallpaper",
                                 "ImageCache"));
            _logger = logger;
            _wallpaperApiClient = wallpaperApiClient;

            _downloaderQueue = new BlockingCollection<LocalWallpaper>();
            _downloader = new WallpaperDownloader(logger,
                                                  _downloaderQueue,
                                                  _cacheDirectory,
                                                  _wallpaperApiClient);
            threadManager.Start(_downloader);
            eventService.Register(this);
        }

        public IMaybe<LocalWallpaper> GetById(Guid id)
        {
            lock (_wallpaper)
            {
                if (!_wallpaper.ContainsKey(id))
                {
                    _logger.Debug("Requesting wallpaper with id '{0}' from server", id);
                    var wallpaper = QueryWallpaper(id);
                    if (wallpaper.HasValue)
                    {
                        RefreshWallpaper(wallpaper.Value);
                    }
                    return wallpaper;
                }
                else
                {
                    _logger.Debug("Requesting wallpaper with id '{0}' from local source", id);
                    var wallpaper = _wallpaper[id];
                    RefreshWallpaper(wallpaper);
                    return new Maybe<LocalWallpaper>(wallpaper);
                }
            }
        }

        public void CountView(LocalWallpaper currentWallpaper)
            => _wallpaperApiClient.PostCommand(new CountWallpaperView
            {
                WallpaperId = currentWallpaper.Id
            });


        private void RefreshWallpaper(LocalWallpaper wallpaper)
        {
            if (wallpaper.Thumbnail == null)
            {
                wallpaper.Thumbnail = new FileInfo(Path.Combine(_cacheDirectory.FullName, wallpaper.Id.ToString() + "_thumb"));
            }
            if (wallpaper.Image == null)
            {
                wallpaper.Image = new FileInfo(Path.Combine(_cacheDirectory.FullName, wallpaper.Id.ToString()));
            }

            if (!_downloaderQueue.IsAddingCompleted)
            {
                _downloaderQueue.Add(wallpaper);
            }
        }

        private IMaybe<LocalWallpaper> QueryWallpaper(Guid id)
        {
            var url = $"wallpaper/wallpaper/{id.ToString()}";
            var result = _wallpaperApiClient.ExecuteApiCall<Web.Wallpapers.Wallpaper>(url).Result;

            return new Maybe<LocalWallpaper>(new LocalWallpaper
            {
                Caption = result.Caption,
                Id = id
            });
        }

        public void Update(SystemIsShutingDownMessage message)
            => _downloaderQueue.CompleteAdding();
    }

    public class WallpaperDownloader : IManagedThread
    {
        private readonly ILogger _logger;
        private readonly BlockingCollection<LocalWallpaper> _collection;
        private readonly DirectoryInfo _cacheDirectory;
        private readonly WallpaperApiClient _wallpaperApiClient;

        public string Name => "WallpaperDownloader";

        public WallpaperDownloader(ILogger logger,
                                   BlockingCollection<LocalWallpaper> collection,
                                   DirectoryInfo cacheDirectory,
                                   WallpaperApiClient wallpaperApiClient)
        {
            _logger = logger;
            _collection = collection;
            _cacheDirectory = cacheDirectory;
            _wallpaperApiClient = wallpaperApiClient;
        }

        public void Run(IManagedThreadHandle handle)
        {
            foreach (var wallpaper in _collection.GetConsumingEnumerable())
            {
                try
                {
                    _logger.Debug("Checking images for wallpaper '{0}'", wallpaper.Id);
                    CheckFile(wallpaper.Image, $"wallpaper/wallpaper/{wallpaper.Id.ToString()}/original");
                    CheckFile(wallpaper.Thumbnail, $"wallpaper/wallpaper/{wallpaper.Id.ToString()}/thumbnail");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Downloading of wallpaper '{0}' not possible", wallpaper.Id);
                }

                if (handle.WasInterrupted) break;
            }
        }

        private void CheckFile(FileInfo fileInfo, string url)
        {
            _logger.Debug("Checking images file '{0}'", fileInfo.FullName);

            if (RefreshNeeded(fileInfo))
            {
                _logger.Debug("Image file '{0}' needs refresh. Query from server '{1}'", fileInfo.FullName, url);
                _wallpaperApiClient.ExecuteStreamCall(url).ContinueWith(x =>
                {
                    using (var writeStream = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        x.Result.CopyTo(writeStream);
                        fileInfo.Refresh();
                    }
                });
            }
            else
            {
                _logger.Debug("Image file '{0}' already existing localy.", fileInfo.FullName);
            }
        }

        private bool RefreshNeeded(FileInfo fileInfo)
        {
            fileInfo.Refresh();

            if (fileInfo.Exists)
            {
                try
                {
                    var bitmap = new Bitmap(fileInfo.FullName);
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.Debug("Reading of file {0} as image not possible: {1}", fileInfo.FullName, ex);
                    fileInfo.Delete();
                }
            }
            else
            {
                _logger.Debug("File {0} does not exists", fileInfo.FullName);
            }

            return true;
        }

        private void EnsureCacheDirectory()
        {
            _cacheDirectory.Refresh();

            if (!_cacheDirectory.Exists)
            {
                _logger.Info("Cache directory '{0}' does not exists. Creating new one");
                _cacheDirectory.Create();
                _cacheDirectory.Refresh();
            }
        }
    }
}
