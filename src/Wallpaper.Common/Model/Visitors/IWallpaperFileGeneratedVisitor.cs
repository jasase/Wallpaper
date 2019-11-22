namespace Plugin.Application.Wallpaper.Common.Model.Visitors
{
    public interface IWallpaperFileGeneratedVisitor
    {
        void Handle(WallpaperFileOriginal wallpaperFile);
        void Handle(WallpaperFileCaption wallpaperFile);
        void Handle(WallpaperFileThumbnail wallpaperFileThumbnail);
    }

    public interface IWallpaperFileGeneratedVisitor<TReturn>
    {
        TReturn Handle(WallpaperFileOriginal wallpaperFile);
        TReturn Handle(WallpaperFileCaption wallpaperFile);
        TReturn Handle(WallpaperFileThumbnail wallpaperFileThumbnail);
    }
}
