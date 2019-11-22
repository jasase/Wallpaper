namespace Plugin.Application.Wallpaper.Common.Model.Configurations
{
    public interface IWallpaperDeployerConfigurationVisitor
    {
        void Handle(OneDriveDeployerConfiguration oneDriveDeployerConfiguration);
    }

    public interface IWallpaperDeployerConfigurationVisitor<TReturn>
    {
        TReturn Handle(OneDriveDeployerConfiguration oneDriveDeployerConfiguration);
    }
}
