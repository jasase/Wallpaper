namespace Plugin.Application.Wallpaper.Activities.WallpaperLoader.Google
{
    public class GoogleEarthImageId : IWallpaperPreLoadDto
    {
        public GoogleEarthImageId(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public string GetImageHash()
            => Id;
    }
}
