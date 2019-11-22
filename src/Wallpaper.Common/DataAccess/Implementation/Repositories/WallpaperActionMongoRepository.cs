using System;
using System.Collections.Generic;
using Framework.Abstraction.Services.DataAccess;
using Framework.Contracts.Services.DataAccess;
using MongoDB.Driver;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Repositories;
using Plugin.Application.Wallpaper.Common.Model;
using Plugin.Application.Wallpaper.Common.Model.Actions;
using Plugin.DataAccess.MongoDb;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Implementation.Repositories
{
    public class WallpaperActionMongoRepository
        : EntityMongoRepository<WallpaperAction>,
          IWallpaperActionRepository
    {
        public WallpaperActionMongoRepository(IMongoDataAccessProvider dataAccessProvider)
            : base(dataAccessProvider)
        { }

        public IEnumerable<WallpaperAction> GetAll(Guid wallpaperId)
            => Collection.Find(Filter().Eq(x => x.WallpaperId, wallpaperId)).ToEnumerable();

        public IEnumerable<TAction> GetForClient<TAction>(Guid clientId)
            where TAction : WallpaperAction
            => Collection.OfType<TAction>()
                         .Find(Filter<TAction>().Eq(x => x.ClientId, clientId))
                         .ToEnumerable();

        public IEnumerable<TAction> GetForClient<TAction>(Guid clientId, Guid[] wallpaperIds)
            where TAction : WallpaperAction
            => Collection.OfType<TAction>()
                         .Find(Filter<TAction>().Eq(x => x.ClientId, clientId) &
                               Filter<TAction>().In(x => x.WallpaperId, wallpaperIds))
                         .ToEnumerable();

        public IEnumerable<TAction> GetForClient<TAction>(Guid clientId, Guid wallpaperId)
            where TAction : WallpaperAction
            => Collection.OfType<TAction>()
                         .Find(Filter<TAction>().Eq(x => x.ClientId, clientId) &
                              Filter<TAction>().Eq(x => x.WallpaperId, wallpaperId))
                         .ToEnumerable();

        public IEnumerable<TAction> GetForWallpaper<TAction>(Guid wallpaperId)
            where TAction : WallpaperAction
            => Collection.OfType<TAction>()
                         .Find(Filter<TAction>().Eq(x => x.WallpaperId, wallpaperId))
                         .ToEnumerable();
    }
}
