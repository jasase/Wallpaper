using Framework.Abstraction.Services.DataAccess;
using Framework.Contracts.Services.DataAccess;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Repositories;
using Plugin.Application.Wallpaper.Common.Model.Configurations;
using Plugin.DataAccess.MongoDb;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Implementation.Repositories
{
    public class WallpaperDeployerConfigurationMongoRepository
        : EntityMongoRepository<WallpaperDeployerConfiguration>, IWallpaperDeployerConfigurationRepository
    {
        public WallpaperDeployerConfigurationMongoRepository(IMongoDataAccessProvider dataAccessProvider)
            : base(dataAccessProvider)
        { }

        public void UpdateRefreshToken(OneDriveDeployerConfiguration configuration)
        {
            var filter = Filter<OneDriveDeployerConfiguration>().Eq(x => x.Id, configuration.Id);
            var update = Update<OneDriveDeployerConfiguration>().Set(x => x.AzureOneDriveRefreshToken, configuration.AzureOneDriveRefreshToken);

            Collection.OfType<OneDriveDeployerConfiguration>().UpdateOne(filter, update);
        }
    }
}
