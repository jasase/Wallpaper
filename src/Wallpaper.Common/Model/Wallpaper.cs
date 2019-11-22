using System;
using System.Collections.Generic;
using Framework.Contracts.Entities;

namespace Plugin.Application.Wallpaper.Common.Model
{
    public class Wallpaper : Entity
    {
        public WallpaperInformation Information { get; set; }
        public List<WallpaperFile> Files { get; set; }
        public Guid RawInformations { get; set; }
        public bool IsTaggedForRefresh { get; set; }

        public Wallpaper()
        {
            Files = new List<WallpaperFile>();
        }
    }
}
