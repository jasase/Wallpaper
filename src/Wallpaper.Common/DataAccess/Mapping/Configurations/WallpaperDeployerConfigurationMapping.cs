using MongoDB.Bson.Serialization;
using Plugin.Application.Wallpaper.Common.Model.Configurations;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Mapping.Configurations
{
    public class WallpaperDeployerConfigurationMapping : BsonClassMap<WallpaperDeployerConfiguration>
    {
        public WallpaperDeployerConfigurationMapping()
        {
            MapProperty(x => x.Active);
            MapProperty(x => x.Name);
        }
    }
}
