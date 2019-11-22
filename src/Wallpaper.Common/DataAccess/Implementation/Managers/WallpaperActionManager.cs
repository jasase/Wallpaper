using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Common.Services.DataAccess;
using Framework.Common.Services.DataAccess.EntityDescriptions;
using Framework.Contracts.Extension;
using Framework.Contracts.Services.DataAccess;
using Framework.Contracts.Services.DataAccess.EntityDescriptions;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Managers;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Repositories;
using Plugin.Application.Wallpaper.Common.Model.Actions;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Implementation.Managers
{
    public class WallpaperActionManager
        : EntityManager<WallpaperAction>,
          IWallpaperActionManager
    {
        private readonly IWallpaperActionRepository _repository;

        public WallpaperActionManager(IWallpaperActionRepository repository,
                                      IEventService eventService)
            : base(repository, eventService)
        {
            _repository = repository;
        }

        IEntityDescription<WallpaperAction> IManager<WallpaperAction>.DescriptionGeneric => throw new NotImplementedException();

        public void CountView(Guid clientId, Guid wallpaperId)
            => _repository.Insert(new WallpaperActionView
            {
                ClientId = clientId,
                Timestamp = DateTime.Now,
                WallpaperId = wallpaperId
            });

        public IEnumerable<WallpaperAction> GetAll(Model.Wallpaper wallpaper)
            => _repository.GetAll(wallpaper.Id);        

        public IEnumerable<WallpaperCountResult> GetViewCount(Guid clientId)
            => ViewToCountResult(_repository.GetForClient<WallpaperActionView>(clientId));

        public IEnumerable<WallpaperCountResult> GetViewCount(Guid clientId, Model.Wallpaper[] filter)
            => ViewToCountResult(_repository.GetForClient<WallpaperActionView>(clientId, filter.Select(x => x.Id).ToArray()));

        public WallpaperCountResult GetViewCount(Guid clientId, Model.Wallpaper wallpaper)
        {
            var result = _repository.GetForClient<WallpaperAction>(clientId, wallpaper.Id);
            return new WallpaperCountResult
            {
                Count = result.Count(),
                WallpaperId = wallpaper.Id
            };
        }

        public WallpaperCountResult GetViewCount(Model.Wallpaper wallpaper)
        {
            var result = _repository.GetForWallpaper<WallpaperAction>(wallpaper.Id);
            return new WallpaperCountResult
            {
                Count = result.Count(),
                WallpaperId = wallpaper.Id
            };
        }

        protected override EntityDescription<WallpaperAction> CreateEntityDescription()
            => new EntityDescription<WallpaperAction>("WallpaperAction",
                                                       new PropertyGroupDescription<WallpaperAction>[0],
                                                       new PropertyDescription<WallpaperAction>[0]);

        private IEnumerable<WallpaperCountResult> ViewToCountResult(IEnumerable<WallpaperActionView> views)
        => views.GroupBy(a => a.WallpaperId)
                .Select(x => new WallpaperCountResult
                {
                    WallpaperId = x.Key,
                    Count = x.Count()
                });
    }
}
