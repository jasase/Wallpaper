using System;
using System.IO;

namespace Plugin.Application.Wallpaper.Client.Model
{
    public class LocalWallpaper
    {
        public Guid Id { get; set; }

        public string Caption { get; set; }

        public bool HasThumbnail => Thumbnail != null && Thumbnail.Exists;
        public FileInfo Thumbnail { get; set; }

        public bool HasImage => Image != null && Image.Exists;
        public FileInfo Image { get; set; }
    }
}
