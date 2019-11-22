using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Common.Helper;
using Framework.Contracts.Helper;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using Plugin.Application.Wallpaper.Common.Model;
using Plugin.Application.Wallpaper.DataAccess.Contracts.Repositories;
using Plugin.MongoDb.Interfaces;

namespace Plugin.Application.Wallpaper.DataAccess.Implementation.Repositories
{
    public class WallpaperDataGridFSRepository : IWallpaperDataRepository
    {
        private const string WALLPAPER_ID_FIELD_NAME = "WallpaperId";
        private readonly IMongoFactory _factory;
        private readonly GridFSBucket<Guid> _fs;

        public WallpaperDataGridFSRepository(IMongoFactory factory)

        {
            _factory = factory;
            _fs = _factory.GetGridFsBucket();
        }

        public bool Delete(Guid id)
        {
            _fs.Delete(id);
            return true;
        }

        public IEnumerable<WallpaperData> GetAll()
        {
            yield break;
        }
        public IEnumerable GetAllAsObjects() => GetAll();

        public IEnumerable<WallpaperData> GetAllDeleted()
        {
            yield break;
        }

        public IEnumerable<WallpaperData> GetById(Guid[] ids)
        {
            foreach (var id in ids)
            {
                var data = GetById(id);
                if (data.HasValue)
                {
                    yield return data.Value;
                }
            }
        }

        public IMaybe<WallpaperData> GetById(Guid id)
        {
            using (var stream = _fs.OpenDownloadStream(id))
            {
                if (stream != null)
                {
                    var data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);

                    var dto = new WallpaperData
                    {
                        Id = id,
                        Data = data,
                    };

                    if (stream.FileInfo.Metadata.IsBsonDocument &&
                        stream.FileInfo.Metadata.TryGetValue(WALLPAPER_ID_FIELD_NAME, out var wallpaperId) &&
                        wallpaperId.IsGuid)
                    {
                        dto.WallpaperId = wallpaperId.AsGuid;
                    }

                    return new Maybe<WallpaperData>(dto);
                }
            }

            return new Maybe<WallpaperData>(null);
        }

        public IMaybe<object> GetByIdAsObject(Guid id) => GetById(id);

        public bool Insert(WallpaperData data)
        {
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument
                {
                    { WALLPAPER_ID_FIELD_NAME , data.WallpaperId }
                }
            };
            _fs.UploadFromBytes(data.Id, data.Id.ToString(), data.Data, options);

            return true;
        }

        public bool Update(WallpaperData data) => throw new NotSupportedException();
    }
}
