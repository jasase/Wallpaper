using System;
using System.Collections.Generic;
using Framework.Contracts.Entities;

namespace Plugin.Application.Wallpaper.Common.Model
{
    public class WallpaperInformation
    {
        public string Caption { get; set; }
        public string Hash { get; set; }
        public WallpaperSource Source { get; set; }
        public List<WallpaperLocation> Locations { get; set; }
        public Uri Url { get;  set; }
        public DateTime Created { get; set; }

        public WallpaperInformation()
        {
            Locations = new List<WallpaperLocation>();
        }
    }
}
