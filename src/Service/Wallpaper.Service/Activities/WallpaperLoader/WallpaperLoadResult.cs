using Plugin.Application.Wallpaper.Common.Model;
using Plugin.Application.Wallpaper.Common.Model.WorkItems;

namespace Plugin.Application.Wallpaper.Activities.WallpaperLoader
{
    public class WallpaperLoadResult
    {
        public WallpaperLoadResult(WallpaperInformation information,
                                   WorkItem rawValues,
                                   WallpaperFileWithData[] data)
        {
            Information = information;
            RawValues = rawValues;
            Data = data;
        }

        public WallpaperInformation Information { get; }
        public WorkItem RawValues { get; }
        public WallpaperFileWithData[] Data { get; }
    }
}
