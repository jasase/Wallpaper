using System.Collections.Generic;
using Framework.Abstraction.Services.DataAccess;
using Plugin.Application.Wallpaper.Common.Model;

namespace Plugin.Application.Wallpaper.DataAccess.Contracts.Repositories
{
    public interface IWallpaperDataRepository
        : IRepository<WallpaperData>
    { }
}
