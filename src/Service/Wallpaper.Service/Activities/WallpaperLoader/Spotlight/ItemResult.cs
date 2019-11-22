namespace Plugin.Application.Wallpaper.Activities.WallpaperLoader.Spotlight
{
#pragma warning disable IDE1006 // Naming Styles
    public class ItemResult : IWallpaperPreLoadDto
    {
        public string v { get; set; }
        public ItemResultAd ad { get; set; }

        public string GetImageHash()
            => ad.image_fullscreen_001_landscape.sha256;
    }

    public class ItemResultAd
    {
        public ItemResultImage image_fullscreen_001_landscape { get; set; }
        public ItemResultText hs1_title_text { get; set; }
        public ItemResultText hs2_title_text { get; set; }
        public ItemResultText title_text { get; set; }
    }

    public class ItemResultImage
    {
        public string t { get; set; }
        public int w { get; set; }
        public int h { get; set; }
        public string u { get; set; }
        public string sha256 { get; set; }
        public int fileSize { get; set; }
    }

    public class ItemResultText
    {
        public string t { get; set; }
        public string tx { get; set; }
    }
#pragma warning restore IDE1006 // Naming Styles
}
