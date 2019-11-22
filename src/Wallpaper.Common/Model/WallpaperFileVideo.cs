using Plugin.Application.Wallpaper.Common.Model.Visitors;

namespace Plugin.Application.Wallpaper.Common.Model
{
    public class WallpaperFileVideo : WallpaperFileOriginal
    {
        public override void Accept(IWallpaperFileVisitor visitor) => visitor.Handle(this);
        public override TReturn Accept<TReturn>(IWallpaperFileVisitor<TReturn> visitor) => visitor.Handle(this);
        public override void Accept(IWallpaperFileOriginalVisitor visitor) => visitor.Handle(this);
        public override TReturn Accept<TReturn>(IWallpaperFileOriginalVisitor<TReturn> visitor) => visitor.Handle(this);        
    }
}
