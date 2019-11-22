using System;
using Plugin.Application.Wallpaper.Common.Model.Visitors;

namespace Plugin.Application.Wallpaper.Common.Model
{
    public abstract class WallpaperFileGenerated : WallpaperFile
    {
        public Guid OriginalFileId { get; set; }

        public override void Accept(IWallpaperFileOriginalVisitor visitor) => visitor.Handle(this);
        public override TReturn Accept<TReturn>(IWallpaperFileOriginalVisitor<TReturn> visitor) => visitor.Handle(this);
    }
}
