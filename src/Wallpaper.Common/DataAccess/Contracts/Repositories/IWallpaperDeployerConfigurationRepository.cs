using Framework.Contracts.Services.DataAccess;
using Plugin.Application.Wallpaper.Common.Model.Configurations;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Repositories
{
    public interface IWallpaperDeployerConfigurationRepository
        : IRepository<WallpaperDeployerConfiguration>
    {
        void UpdateRefreshToken(OneDriveDeployerConfiguration configuration);
    }
}
