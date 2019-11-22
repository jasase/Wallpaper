using Plugin.Application.Wallpaper.Common.Model.Visitors;

namespace Plugin.Application.Wallpaper.Common.Model
{
    public class WallpaperFileThumbnail : WallpaperFileGenerated
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public override void Accept(IWallpaperFileVisitor visitor) => visitor.Handle(this);
        public override TReturn Accept<TReturn>(IWallpaperFileVisitor<TReturn> visitor) => visitor.Handle(this);
        public override void Accept(IWallpaperFileGeneratedVisitor visitor) => visitor.Handle(this);
        public override TReturn Accept<TReturn>(IWallpaperFileGeneratedVisitor<TReturn> visitor) => visitor.Handle(this);
    }
}
