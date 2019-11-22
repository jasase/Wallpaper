using System;
using System.Collections.Generic;
using Framework.Contracts.Helper;
using Framework.Contracts.Services.DataAccess;
using Plugin.Application.Wallpaper.Common.Model;

namespace Plugin.Application.Wallpaper.DataAccess.Contracts.Managers
{
    public interface IWallpaperManager : IManager<Common.Model.Wallpaper>
    {
        Common.Model.Wallpaper Insert(WallpaperInformation information, Guid idOfRawValue, WallpaperFileWithData[] files);

        bool Exists(WallpaperSource source, string hash);
        IMaybe<Common.Model.Wallpaper> GetByHashAndSource(WallpaperSource source, string hash);

        IMaybe<WallpaperData> GetFile(Common.Model.Wallpaper wallpaper, WallpaperFile original);
        Common.Model.Wallpaper DeleteFile(Common.Model.Wallpaper wallpaper, WallpaperFile generatedFile);
        Common.Model.Wallpaper AddFile(Common.Model.Wallpaper wallpaper, WallpaperFileWithData file);

        IEnumerable<Common.Model.Wallpaper> GetLastAdded(int count);

    }
}
