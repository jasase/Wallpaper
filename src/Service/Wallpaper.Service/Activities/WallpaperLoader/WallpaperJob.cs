using System;
using Framework.Abstraction.Extension;
using Framework.Abstraction.Services.Scheduling;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Managers;
using Plugin.Application.Wallpaper.DataAccess.Contracts.Managers;

namespace Plugin.Application.Wallpaper.Activities.WallpaperLoader
{
    public class WallpaperJob : IJob
    {
        private readonly WallpaperLoaderHandler[] _loaders;
        private readonly IWallpaperManager _wallpaperManager;
        private readonly IWorkItemManager _workItemManager;
        private readonly ILogger _logger;

        public WallpaperJob(WallpaperLoaderHandler[] loaders,
                            IWallpaperManager wallpaperManager,
                            IWorkItemManager workItemManager,
                            ILogger logger)
        {
            _loaders = loaders;
            _wallpaperManager = wallpaperManager;
            _workItemManager = workItemManager;
            _logger = logger;
        }

        public string Name => "WallpaperLoader";

        public void Execute()
        {
            foreach (var loader in _loaders)
            {
                _logger.Info("Starting loading images from source {0}", loader.SourceName);
                try
                {
                    loader.LoadWallpapers();                  
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error occurred during loading images from source {0}", loader.SourceName);
                }
            }
        }
    }
}
