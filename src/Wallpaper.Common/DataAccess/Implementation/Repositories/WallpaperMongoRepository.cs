using System.Collections.Generic;
using Framework.Common.Helper;
using Framework.Contracts.Helper;
using Framework.Contracts.Services.DataAccess;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Plugin.Application.Wallpaper.Common.Model;
using Plugin.Application.Wallpaper.DataAccess.Contracts.Repositories;
using PluginMongoDb;

namespace Plugin.Application.Wallpaper.DataAccess.Implementation.Repositories
{
    public class WallpaperMongoRepository : EntityMongoRepository<Common.Model.Wallpaper>, IWallpaperRepository
    {
        private readonly IMongoCollection<BsonDocument> _t;

        public WallpaperMongoRepository(IMongoDataAccessProvider dataAccessProvider)
            : base(dataAccessProvider)
        {
            _t = dataAccessProvider.MongoFactory.GetOrCreateCollectionBson<Common.Model.Wallpaper>();
        }

        public Common.Model.Wallpaper AddFile(Common.Model.Wallpaper wallpaper, WallpaperFile file)
            => Collection.FindOneAndUpdate(Filter().Eq(x => x.Id, wallpaper.Id),
                                           Update().Push(x => x.Files, file));

        public Common.Model.Wallpaper DeleteFile(Common.Model.Wallpaper wallpaper, WallpaperFile file)
         => Collection.FindOneAndUpdate(Filter().Eq(x => x.Id, wallpaper.Id),
                                           Update().PullFilter(x => x.Files, Builders<WallpaperFile>.Filter.Eq(x => x.FileId, file.FileId)));

        public bool Exists(string sourceName, string hash)
        {
            var result = Collection.FindSync(Filter().Eq(x => x.Information.Source.Name, sourceName) &
                                             Filter().Eq(x => x.Information.Hash, hash) &
                                             (Filter().Eq(x => x.IsDeleted, false) | !Filter().Exists(x => x.IsDeleted)))
                                   .Any();

            return result;
        }

        public IMaybe<Common.Model.Wallpaper> GetByHash(string sourceName, string hash)
        {
            var result = Collection.FindSync(Filter().Eq(x => x.Information.Source.Name, sourceName) &
                                             Filter().Eq(x => x.Information.Hash, hash) &
                                             (Filter().Eq(x => x.IsDeleted, false) | !Filter().Exists(x => x.IsDeleted)))
                                   .FirstOrDefault();

            return new Maybe<Common.Model.Wallpaper>(result);
        }

        public IEnumerable<Common.Model.Wallpaper> GetLastAdded(int count)
        {
            var result = Collection.Find(Filter().Eq(x => x.IsDeleted, false))
                                   .SortByDescending(x => x.Information.Created)
                                   .Limit(count);

            return result.ToEnumerable();
        }
    }
}
