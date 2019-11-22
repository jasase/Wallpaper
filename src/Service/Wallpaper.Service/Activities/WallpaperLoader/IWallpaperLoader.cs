using System.Collections.Generic;
using Plugin.Application.Wallpaper.Common.Model;

namespace Plugin.Application.Wallpaper.Activities.WallpaperLoader
{
    public interface IWallpaperLoader<TWallpaperDto>
         where TWallpaperDto : IWallpaperPreLoadDto
    {
        WallpaperSource Source { get; }

        IEnumerable<TWallpaperDto> LoadAvailiableImages();

        WallpaperLoadResult LoadWallpaper(TWallpaperDto wallpaper);
    }
}
