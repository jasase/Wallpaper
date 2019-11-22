using Framework.Abstraction.Services.DataAccess;
using Plugin.Application.Wallpaper.Common.Model.Configurations;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Managers
{
    public interface IWallpaperDeployerConfigurationManager
        : IManager<WallpaperDeployerConfiguration>
    {
        void UpdateRefreshToken(OneDriveDeployerConfiguration configuration);
    }
}
