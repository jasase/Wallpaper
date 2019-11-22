namespace Plugin.Application.Wallpaper.Common.Model.Visitors
{
    public interface IWallpaperFileVisitor
    {
        void Handle(WallpaperFileVideo wallpaperFile);
        void Handle(WallpaperFileImage wallpaperFile);
        void Handle(WallpaperFileCaption wallpaperFile);
        void Handle(WallpaperFileThumbnail wallpaperFileThumbnail);
    }

    public interface IWallpaperFileVisitor<TReturn>
    {
        TReturn Handle(WallpaperFileVideo wallpaperFile);
        TReturn Handle(WallpaperFileImage wallpaperFile);
        TReturn Handle(WallpaperFileThumbnail wallpaperFileThumbnail);
        TReturn Handle(WallpaperFileCaption wallpaperFile);
    }
}
