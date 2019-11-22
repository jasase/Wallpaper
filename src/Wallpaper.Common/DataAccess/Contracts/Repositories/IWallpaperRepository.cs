using System.Collections.Generic;
using Framework.Contracts.Helper;
using Framework.Contracts.Services.DataAccess;
using Plugin.Application.Wallpaper.Common.Model;

namespace Plugin.Application.Wallpaper.DataAccess.Contracts.Repositories
{
    public interface IWallpaperRepository : IRepository<Common.Model.Wallpaper>
    {
        bool Exists(string sourceName, string hash);
        IMaybe<Common.Model.Wallpaper> GetByHash(string sourceName, string hash);
        Common.Model.Wallpaper AddFile(Common.Model.Wallpaper wallpaper, WallpaperFile file);
        Common.Model.Wallpaper DeleteFile(Common.Model.Wallpaper wallpaper, WallpaperFile file);
        IEnumerable<Common.Model.Wallpaper> GetLastAdded(int count);
    }
}
