namespace Plugin.Application.Wallpaper.Common.Model.Visitors
{
    public interface IWallpaperLocationVisitor
    {
        void Handle(WallpaperLocationCoordinates wallpaperLocation);
        void Handle(WallpaperLocationAddress wallpaperLocation);
    }
    public interface IWallpaperLocationVisitor<TReturn>
    {
        TReturn Handle(WallpaperLocationCoordinates wallpaperLocation);
        TReturn Handle(WallpaperLocationAddress wallpaperLocation);
    }

}
