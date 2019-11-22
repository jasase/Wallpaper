namespace Plugin.Application.Wallpaper.Common.Model.Configurations
{
    public class OneDriveDeployerConfiguration : WallpaperDeployerConfiguration
    {
        public string AzureOneDriveRefreshToken { get; set; }
        public override void Accept(IWallpaperDeployerConfigurationVisitor visitor)
            => visitor.Handle(this);

        public override TReturn Accept<TReturn>(IWallpaperDeployerConfigurationVisitor<TReturn> visitor)
             => visitor.Handle(this);
    }
}
