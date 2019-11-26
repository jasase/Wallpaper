using Framework.Abstraction.Extension;
using Framework.Abstraction.Services.Scheduling;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Managers;
using Plugin.Application.Wallpaper.DataAccess.Contracts.Managers;

namespace Plugin.Application.Wallpaper.Activities.Cleaners
{
    public class WallpaperDeletedCleaner : IJob
    {
        private readonly ILogger _logger;
        private readonly IWallpaperManager _wallpaperManager;
        private readonly IWallpaperActionManager _wallpaperActionManager;
        private readonly IWorkItemManager _workItemManager;

        public WallpaperDeletedCleaner(ILogger logger,
                                       IWallpaperManager wallpaperManager,
                                       IWallpaperActionManager wallpaperActionManager,
                                       IWorkItemManager workItemManager)
        {
            _logger = logger;
            _wallpaperManager = wallpaperManager;
            _wallpaperActionManager = wallpaperActionManager;
            _workItemManager = workItemManager;
        }

        public string Name => "WallpaperDeletedCleaner";

        public void Execute()
        {
            _logger.Info("Cleaning up deleted wallpapers");

            foreach (var deleted in _wallpaperManager.GetAllDeleted())
            {
                _logger.Debug("Cleaning up wallpaper '{0}'", deleted.Id);
                foreach (var file in deleted.Files)
                {
                    _wallpaperManager.DeleteFile(deleted, file);
                }

                _workItemManager.Delete(deleted.RawInformations);

                foreach (var action in _wallpaperActionManager.GetAll(deleted))
                {
                    _wallpaperActionManager.Delete(action.Id);
                }
            }
        }
    }
}
