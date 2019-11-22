using System.Collections.Generic;
using Framework.Contracts.Services.DataAccess;
using Plugin.Application.Wallpaper.Common.Model;

namespace Plugin.Application.Wallpaper.DataAccess.Contracts.Repositories
{
    public interface IWallpaperDataRepository
        : IRepository<WallpaperData>
    { }
}
