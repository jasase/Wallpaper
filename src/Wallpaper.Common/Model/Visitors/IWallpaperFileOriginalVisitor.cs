namespace Plugin.Application.Wallpaper.Common.Model.Visitors
{
    public interface IWallpaperFileOriginalVisitor
    {
        void Handle(WallpaperFileGenerated wallpaperFile);
        void Handle(WallpaperFileVideo wallpaperFile);
        void Handle(WallpaperFileImage wallpaperFile);
    }

    public interface IWallpaperFileOriginalVisitor<TReturn>
    {
        TReturn Handle(WallpaperFileGenerated wallpaperFile);
        TReturn Handle(WallpaperFileVideo wallpaperFile);
        TReturn Handle(WallpaperFileImage wallpaperFile);
    }
}
