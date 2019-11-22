using System;
using System.Collections.Generic;
using Framework.Abstraction.Services.DataAccess;
using Plugin.Application.Wallpaper.Common.Model.Actions;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Repositories
{
    public interface IWallpaperActionRepository
        : IRepository<WallpaperAction>
    {
        IEnumerable<TAction> GetForWallpaper<TAction>(Guid wallpaperId)
           where TAction : WallpaperAction;

        IEnumerable<TAction> GetForClient<TAction>(Guid clientId)
            where TAction : WallpaperAction;
        IEnumerable<TAction> GetForClient<TAction>(Guid clientId, Guid[] wallpaperIds)
            where TAction : WallpaperAction;
        IEnumerable<TAction> GetForClient<TAction>(Guid clientId, Guid wallpaperId)
            where TAction : WallpaperAction;
        IEnumerable<WallpaperAction> GetAll(Guid wallpaperId);
    }
}
