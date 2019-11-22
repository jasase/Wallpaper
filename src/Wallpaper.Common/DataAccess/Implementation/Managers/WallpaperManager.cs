using System;
using System.Collections.Generic;
using Framework.Core.Helper;
using Framework.Core.Services.DataAccess;
using Framework.Core.Services.DataAccess.EntityDescriptions;
using Framework.Abstraction.Extension;
using Framework.Abstraction.Helper;
using Framework.Abstraction.Messages.EntityMessages;
using Plugin.Application.Wallpaper.Common.Model;
using Plugin.Application.Wallpaper.DataAccess.Contracts.Managers;
using Plugin.Application.Wallpaper.DataAccess.Contracts.Repositories;

namespace Plugin.Application.Wallpaper.DataAccess.Implementation.Managers
{
    public class WallpaperManager : EntityManager<Common.Model.Wallpaper>, IWallpaperManager
    {
        private readonly IWallpaperRepository _repository;
        private readonly IWallpaperDataRepository _dataRepository;

        public WallpaperManager(IWallpaperRepository repository,
                                IWallpaperDataRepository dataRepository,
                                IEventService eventService)
            : base(repository, eventService)
        {
            _repository = repository;
            _dataRepository = dataRepository;
        }

        public Common.Model.Wallpaper AddFile(Common.Model.Wallpaper wallpaper, WallpaperFileWithData file)
        {
            file.FileDto.FileId = Guid.NewGuid();
            _dataRepository.Insert(new WallpaperData
            {
                Data = file.Data,
                WallpaperId = wallpaper.Id,
                Id = file.FileDto.FileId
            });

            return _repository.AddFile(wallpaper, file.FileDto);
        }

        public Common.Model.Wallpaper DeleteFile(Common.Model.Wallpaper wallpaper, WallpaperFile file)
        {
            if (wallpaper.Files.Contains(file))
            {
                _dataRepository.Delete(file.FileId);
                return _repository.DeleteFile(wallpaper, file);
            }
            return wallpaper;
        }

        public bool Exists(WallpaperSource source, string hash)
            => _repository.Exists(source.Name, hash);

        public IMaybe<Common.Model.Wallpaper> GetByHashAndSource(WallpaperSource source, string hash)
            => _repository.GetByHash(source.Name, hash);

        public IMaybe<WallpaperData> GetFile(Common.Model.Wallpaper wallpaper, WallpaperFile original)
        {
            if (wallpaper.Files.Contains(original))
            {
                var result = _dataRepository.GetById(original.FileId);
                return new Maybe<WallpaperData>(result.Value);
            }
            return new Maybe<WallpaperData>(null);
        }

        public IEnumerable<Common.Model.Wallpaper> GetLastAdded(int count)
            => _repository.GetLastAdded(count);


        public override bool Insert(Common.Model.Wallpaper data)
        {
            data.Information.Created = DateTime.Now;
            return Insert(data.Information, Guid.Empty, new WallpaperFileWithData[0]) != null;
        }

        public Common.Model.Wallpaper Insert(WallpaperInformation information, Guid idOfRawValue, WallpaperFileWithData[] files)
        {
            information.Created = DateTime.Now;

            var entity = new Common.Model.Wallpaper
            {
                Information = information,
                RawInformations = idOfRawValue
            };

            foreach (var file in files)
            {
                file.FileDto.FileId = Guid.NewGuid();
                entity.Files.Add(file.FileDto);

                _dataRepository.Insert(new WallpaperData
                {
                    Data = file.Data,
                    WallpaperId = entity.Id,
                    Id = file.FileDto.FileId
                });
            }

            var result = _repository.Insert(entity);
            if (result) EventService.Publish(new EntityInsertedMessage(typeof(Common.Model.Wallpaper), entity.Id));

            return entity;
        }

        public override bool Delete(Guid id)
        {
            var wallpaper = _repository.GetById(id);
            if (wallpaper.HasValue)
            {
                foreach (var file in wallpaper.Value.Files)
                {
                    _dataRepository.Delete(file.FileId);
                }
            }
            return base.Delete(id);
        }

        protected override EntityDescription<Common.Model.Wallpaper> CreateEntityDescription()
            => new EntityDescription<Common.Model.Wallpaper>("Wallpaper",
                                                       new PropertyGroupDescription<Common.Model.Wallpaper>[0],
                                                       new PropertyDescription<Common.Model.Wallpaper>[0]);
    }
}
