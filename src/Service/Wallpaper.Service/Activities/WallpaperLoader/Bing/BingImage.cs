namespace Plugin.Application.Wallpaper.Activities.WallpaperLoader.Bing
{
    public class BingImage : IWallpaperPreLoadDto
    {
#pragma warning disable IDE1006 // Benennungsstile
        public string url { get; set; }
        public string copyright { get; set; }
        public string hsh { get; set; }
#pragma warning restore IDE1006 // Benennungsstile


        public string GetImageHash() => hsh;
    }
}
