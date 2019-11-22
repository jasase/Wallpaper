using System.Collections.Concurrent;
using Framework.Contracts.Extension;
using Framework.Contracts.Services.ThreadManaging;
using Plugin.Application.Wallpaper.DataAccess.Contracts.Managers;

namespace Plugin.Application.Wallpaper.Activities.ImageConverter
{
    public class ImageConverterThread : IManagedThread
    {
        private readonly BlockingCollection<ImageConverterQueueElement> _inputQueue;
        private readonly ILogger _logger;
        private readonly IWallpaperManager _wallpaperManager;

        public string Name => "ImageConverter";

        public ImageConverterThread(BlockingCollection<ImageConverterQueueElement> inputQueue,
                                    ILogger logger,
                                    IWallpaperManager wallpaperManager)
        {
            _inputQueue = inputQueue ?? throw new System.ArgumentNullException(nameof(inputQueue));
            _logger = logger;
            _wallpaperManager = wallpaperManager ?? throw new System.ArgumentNullException(nameof(wallpaperManager));
        }

        public void Run(IManagedThreadHandle handle)
        {
            foreach (var input in _inputQueue.GetConsumingEnumerable())
            {                
                var wallpaperMaybe = _wallpaperManager.GetById(input.WallpaperId);
                if (!wallpaperMaybe.HasValue)
                {
                    continue;
                }
                var wallpaper = wallpaperMaybe.Value;
                var handler = new ImageConverterWallpaperHandler(_logger, wallpaper, _wallpaperManager);
                foreach (var file in wallpaper.Files)
                {
                    file.Accept(handler);
                }

                handler.ProcessFiles();
            }
        }
    }
}
