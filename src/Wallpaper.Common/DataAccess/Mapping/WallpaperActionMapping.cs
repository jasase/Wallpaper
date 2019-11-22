using MongoDB.Bson.Serialization;
using Plugin.Application.Wallpaper.Common.Model.Actions;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Mapping
{
    public class WallpaperActionMapping : BsonClassMap<WallpaperAction>
    {
        public WallpaperActionMapping()
        {
            SetDiscriminatorIsRequired(true);
            MapProperty(x => x.Timestamp);
            MapProperty(x => x.ClientId);
            MapProperty(x => x.WallpaperId);
        }
    }

    public class WallpaperActionViewMapping : BsonClassMap<WallpaperActionView>
    {
        public WallpaperActionViewMapping()
        { }
    }

}
