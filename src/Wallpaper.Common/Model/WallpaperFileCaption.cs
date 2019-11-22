using Plugin.Application.Wallpaper.Common.Model.Visitors;

namespace Plugin.Application.Wallpaper.Common.Model
{
    public class WallpaperFileCaption : WallpaperFileGenerated
    {
        public WallpaperFileThumbnailPosition Position { get; set; }

        public override void Accept(IWallpaperFileVisitor visitor) => visitor.Handle(this);
        public override TReturn Accept<TReturn>(IWallpaperFileVisitor<TReturn> visitor) => visitor.Handle(this);
        public override void Accept(IWallpaperFileGeneratedVisitor visitor) => visitor.Handle(this);
        public override TReturn Accept<TReturn>(IWallpaperFileGeneratedVisitor<TReturn> visitor) => visitor.Handle(this);


        public enum WallpaperFileThumbnailPosition
        {
            Top,
            Bottom,
            Left,
            Right
        }
    }
}
