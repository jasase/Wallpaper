using MongoDB.Bson.Serialization;
using Plugin.Application.Wallpaper.Common.Model.Configurations;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Mapping.Configurations
{
    public class OneDriveDeployerConfigurationMapping : BsonClassMap<OneDriveDeployerConfiguration>
    {
        public OneDriveDeployerConfigurationMapping()
        {
            MapProperty(x => x.AzureOneDriveRefreshToken);
        }
    }
}
