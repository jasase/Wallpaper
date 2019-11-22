using System;
using Framework.Abstraction.Entities;

namespace Plugin.Application.Wallpaper.Common.Model.Actions
{
    public abstract class WallpaperAction : Entity
    {
        public DateTime Timestamp { get; set; }
        public Guid ClientId { get; set; }
        public Guid WallpaperId { get; set; }
    }
}
