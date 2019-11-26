using Plugin.Application.Wallpaper.Activities.WallpaperDeployers;
using Plugin.Application.Wallpaper.DataAccess.Contracts.Managers;
using System.IO;

namespace Wallpaper.Service.Activities.WallpaperDeployers
{
    public class FolderDeployer : IWallpaperDeployer
    {
        private readonly WallpaperSetting _setting;
        private readonly IWallpaperManager _wallpaperManager;

        public FolderDeployer(WallpaperSetting setting,
                              IWallpaperManager wallpaperManager)
        {
            _setting = setting;
            _wallpaperManager = wallpaperManager;
        }

        public void Deploy(Plugin.Application.Wallpaper.Common.Model.Wallpaper wallpaper)
        {
            var deployFolder = GetFolder();

            foreach (var file in wallpaper.Files)
            {
                var data = _wallpaperManager.GetFile(wallpaper, file);
                if (data.HasValue)
                {
                    var fileInfo = new FileInfo(Path.Combine(deployFolder.FullName, wallpaper.Id + "_" + file.FileId + ".jpg"));
                    if (fileInfo.Exists)
                        continue;

                    using (var stream = fileInfo.OpenWrite())
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(data.Value.Data);
                    }
                }
            }
        }

        private DirectoryInfo GetFolder()
        {
            var directory = new DirectoryInfo(_setting.DeployFolder);
            if (!directory.Exists)
            {
                directory.Create();
                directory.Refresh();
            }

            return directory;
        }
    }
}
