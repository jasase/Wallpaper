using System;
using MongoDB.Bson.Serialization;

namespace Plugin.Application.Wallpaper.DataAccess.Mapping
{
    public class WallpaperMongoMapping : BsonClassMap<Common.Model.Wallpaper>
    {
        public WallpaperMongoMapping()
        {
            MapProperty(x => x.Files);
            MapProperty(x => x.Information);
            MapProperty(x => x.RawInformations)
            .SetDefaultValue(Guid.Empty);
            MapProperty(x => x.IsTaggedForRefresh)
            .SetDefaultValue(true);
        }
    }
}
