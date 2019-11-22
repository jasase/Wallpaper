using System;
using Plugin.Application.Wallpaper.Common.Model.Visitors;

namespace Plugin.Application.Wallpaper.Common.Model
{
    public abstract class WallpaperFile
    {
        public Guid FileId { get; set; }

        public abstract void Accept(IWallpaperFileVisitor visitor);
        public abstract TReturn Accept<TReturn>(IWallpaperFileVisitor<TReturn> visitor);
        public abstract void Accept(IWallpaperFileOriginalVisitor visitor);
        public abstract TReturn Accept<TReturn>(IWallpaperFileOriginalVisitor<TReturn> visitor);
        public abstract void Accept(IWallpaperFileGeneratedVisitor visitor);
        public abstract TReturn Accept<TReturn>(IWallpaperFileGeneratedVisitor<TReturn> visitor);
    }
}
