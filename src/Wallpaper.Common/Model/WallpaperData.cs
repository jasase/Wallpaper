using System;
using Framework.Contracts.Entities;

namespace Plugin.Application.Wallpaper.Common.Model
{
    public class WallpaperData : Entity
    {
        public Guid WallpaperId { get; set; }
        public byte[] Data { get; set; }
    }
}
