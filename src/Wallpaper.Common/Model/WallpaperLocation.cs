using Plugin.Application.Wallpaper.Common.Model.Visitors;

namespace Plugin.Application.Wallpaper.Common.Model
{
    public abstract class WallpaperLocation
    {
        public abstract void Accept(IWallpaperLocationVisitor visitor);
        public abstract TReturn Accept<TReturn>(IWallpaperLocationVisitor<TReturn> visitor);
    }

    public class WallpaperLocationCoordinates : WallpaperLocation
    {
        public double Longtitude { get; set; }
        public double Latitude { get; set; }

        public override void Accept(IWallpaperLocationVisitor visitor) => visitor.Handle(this);
        public override TReturn Accept<TReturn>(IWallpaperLocationVisitor<TReturn> visitor) => visitor.Handle(this);
    }

    public class WallpaperLocationAddress : WallpaperLocation
    {
        public string Country { get; set; }
        public string[] AdministrativeArea { get; set; }
        public string City { get; set; }
        public string[] SubCity { get; set; }
        public string Street { get; set; }

        public override void Accept(IWallpaperLocationVisitor visitor) => visitor.Handle(this);
        public override TReturn Accept<TReturn>(IWallpaperLocationVisitor<TReturn> visitor) => visitor.Handle(this);
    }
}
