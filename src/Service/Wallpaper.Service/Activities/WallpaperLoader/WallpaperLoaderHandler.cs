using System;
using System.Collections.Generic;
using Framework.Abstraction.Extension;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Managers;
using Plugin.Application.Wallpaper.DataAccess.Contracts.Managers;

namespace Plugin.Application.Wallpaper.Activities.WallpaperLoader
{
    public abstract class WallpaperLoaderHandler
    {
        public abstract string SourceName { get; }
        public abstract void LoadWallpapers();
    }

    public class WallpaperLoaderHandler<TWallpaperDto> : WallpaperLoaderHandler
        where TWallpaperDto : IWallpaperPreLoadDto
    {
        private readonly IWallpaperLoader<TWallpaperDto> _loader;
        private readonly IWallpaperManager _wallpaperManager;
        private readonly IWorkItemManager _workItemManager;
        private readonly ILogger _logger;

        public WallpaperLoaderHandler(IWallpaperLoader<TWallpaperDto> loader,
                                      IWallpaperManager wallpaperManager,
                                      IWorkItemManager workItemManager,
                                      ILogger logger)
        {
            _loader = loader;
            _wallpaperManager = wallpaperManager;
            _workItemManager = workItemManager;
            _logger = logger;
        }

        public override string SourceName => _loader.Source.Name;

        public override void LoadWallpapers()
        {
            _logger.Debug("Loading availiable images from source {0}", SourceName);
            var images = _loader.LoadAvailiableImages();
            foreach (var image in images)
            {
                try
                {
                    var mayExistingWallpaper = _wallpaperManager.GetByHashAndSource(_loader.Source, image.GetImageHash());
                    if (mayExistingWallpaper.HasValue && !mayExistingWallpaper.Value.IsTaggedForRefresh)
                    {
                        _logger.Debug("Skipping image [Hash:{0}] of source {1} because it already exists", image.GetImageHash(), SourceName);
                        continue;
                    }

                    _logger.Debug("Downloading image [Hash:{0}] of source {1}", image.GetImageHash(), SourceName);
                    var result = _loader.LoadWallpaper(image);
                    if (result != null)
                    {
                        result.Information.Source = _loader.Source;

                        _logger.Debug("Saving new image [Hash:{0}] of source {1}", result.Information.Hash, result.Information.Source);
                        _workItemManager.Insert(result.RawValues);
                        _wallpaperManager.Insert(result.Information, result.RawValues.Id, result.Data);

                        if (mayExistingWallpaper.HasValue)
                        {
                            _wallpaperManager.Delete(mayExistingWallpaper.Value.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error during downloading image of source '{source}' with hash '{hash}' failed", _loader.Source.Name, image.GetImageHash());
                }
            }
        }
    }
}
