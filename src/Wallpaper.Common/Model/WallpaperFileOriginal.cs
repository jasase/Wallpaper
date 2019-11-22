using Plugin.Application.Wallpaper.Common.Model.Visitors;

namespace Plugin.Application.Wallpaper.Common.Model
{
    public abstract class WallpaperFileOriginal : WallpaperFile
    {
        public override void Accept(IWallpaperFileGeneratedVisitor visitor) => visitor.Handle(this);
        public override TReturn Accept<TReturn>(IWallpaperFileGeneratedVisitor<TReturn> visitor) => visitor.Handle(this);
    }
}
