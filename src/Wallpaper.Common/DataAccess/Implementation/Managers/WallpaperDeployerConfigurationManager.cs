using System;
using Framework.Core.Services.DataAccess;
using Framework.Core.Services.DataAccess.EntityDescriptions;
using Framework.Abstraction.Extension;
using Framework.Abstraction.Services.DataAccess;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Managers;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Repositories;
using Plugin.Application.Wallpaper.Common.Model.Configurations;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Implementation.Managers
{
    public class WallpaperDeployerConfigurationManager
        : EntityManager<WallpaperDeployerConfiguration>, IWallpaperDeployerConfigurationManager
    {
        private readonly IWallpaperDeployerConfigurationRepository _repository;

        public WallpaperDeployerConfigurationManager(IWallpaperDeployerConfigurationRepository repository, IEventService eventService)
            : base(repository, eventService)
        {
            _repository = repository;
        }

        public void UpdateRefreshToken(OneDriveDeployerConfiguration configuration)
            => _repository.UpdateRefreshToken(configuration);

        protected override EntityDescription<WallpaperDeployerConfiguration> CreateEntityDescription()
        => new EntityDescription<WallpaperDeployerConfiguration>("WallpaperDeployerConfiguration",
                                                       new PropertyGroupDescription<WallpaperDeployerConfiguration>[0],
                                                       new PropertyDescription<WallpaperDeployerConfiguration>[0]);
    }
}
