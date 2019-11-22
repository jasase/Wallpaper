using MongoDB.Bson.Serialization;
using Plugin.Application.Wallpaper.Common.Model;

namespace Plugin.Application.Wallpaper.DataAccess.Mapping
{
    public class WallpaperLocationMapping : BsonClassMap<WallpaperLocation>
    { }
    public class WallpaperLocationCoordinatesMapping : BsonClassMap<WallpaperLocationCoordinates>
    {
        public WallpaperLocationCoordinatesMapping()
        {
            MapProperty(x => x.Longtitude);
            MapProperty(x => x.Latitude);            
        }
    }
    public class WallpaperLocationAddressMapping : BsonClassMap<WallpaperLocationAddress>
    {
        public WallpaperLocationAddressMapping()
        {
            MapProperty(x => x.Country);
            MapProperty(x => x.AdministrativeArea);
            MapProperty(x => x.City);
            MapProperty(x => x.SubCity);
            MapProperty(x => x.Street);
        }
    }
}
