using System;
using System.Collections.Generic;
using Framework.Abstraction.Services.DataAccess;
using Plugin.Application.Wallpaper.Common.Model.Actions;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Managers
{
    public interface IWallpaperActionManager
        : IManager<WallpaperAction>
    {

        IEnumerable<WallpaperAction> GetAll(Model.Wallpaper wallpaper);

        void CountView(Guid clientId, Guid wallpaperId);

        IEnumerable<WallpaperCountResult> GetViewCount(Guid clientId);
        IEnumerable<WallpaperCountResult> GetViewCount(Guid clientId, Model.Wallpaper[] filter);
        WallpaperCountResult GetViewCount(Guid clientId, Model.Wallpaper wallpaper);
        WallpaperCountResult GetViewCount(Model.Wallpaper wallpaper);
    }

    public class WallpaperCountResult
    {
        public Guid WallpaperId { get; set; }
        public int Count { get; set; }
    }
}
