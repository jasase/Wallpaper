using MongoDB.Bson.Serialization;
using Plugin.Application.Wallpaper.Common.Model;

namespace Plugin.Application.Wallpaper.DataAccess.Mapping
{
    public class WallpaperFileMapping : BsonClassMap<WallpaperFile>
    {
        public WallpaperFileMapping()
        {
            SetDiscriminatorIsRequired(true);
            MapProperty(x => x.FileId);
        }
    }

    public class WallpaperFileGeneratedMapping : BsonClassMap<WallpaperFileGenerated>
    {
        public WallpaperFileGeneratedMapping()
        {
            MapProperty(x => x.OriginalFileId);
        }
    }

    public class WallpaperFileOriginalMapping : BsonClassMap<WallpaperFileOriginal>
    { }

    public class WallpaperFileCaptionMapping : BsonClassMap<WallpaperFileCaption>
    {
        public WallpaperFileCaptionMapping()
        {
            MapProperty(x => x.Position);
        }
    }

    public class WallpaperFileThumbnailMapping : BsonClassMap<WallpaperFileThumbnail>
    {
        public WallpaperFileThumbnailMapping()
        {
            MapProperty(x => x.Height);
            MapProperty(x => x.Width);
        }
    }

    public class WallpaperFileVideoMapping : BsonClassMap<WallpaperFileVideo>
    { }

    public class WallpaperFileImageMapping : BsonClassMap<WallpaperFileImage>
    {
        public WallpaperFileImageMapping()
        {
            MapProperty(x => x.Width);
            MapProperty(x => x.Height);
            MapProperty(x => x.Format);
        }
    }
}
