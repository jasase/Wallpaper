using System.Collections.Generic;
using Framework.Contracts.Extension;
using Framework.Contracts.IocContainer;
using Framework.Contracts.IocContainer.Registrations;
using Framework.Contracts.Plugins;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Managers;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Repositories;
using Plugin.Application.Wallpaper.Common.DataAccess.Implementation.Managers;
using Plugin.Application.Wallpaper.Common.DataAccess.Implementation.Repositories;
using Plugin.Application.Wallpaper.Common.Model.Actions;
using Plugin.Application.Wallpaper.Common.Model.Clients;
using Plugin.Application.Wallpaper.Common.Model.Configurations;
using Plugin.Application.Wallpaper.Common.Model.WorkItems;
using Plugin.Application.Wallpaper.DataAccess.Contracts.Managers;
using Plugin.Application.Wallpaper.DataAccess.Contracts.Repositories;
using Plugin.Application.Wallpaper.DataAccess.Implementation.Managers;
using Plugin.Application.Wallpaper.DataAccess.Implementation.Repositories;
using Plugin.MongoDb.Interfaces;

namespace Plugin.Application.Wallpaper.Common
{
    public class WallpaperCommonPlugin : Framework.Contracts.Plugins.Plugin, IGeneralPlugin
    {
        public override PluginDescription Description { get; }

        public WallpaperCommonPlugin(IDependencyResolver resolver,
                               IDependencyResolverConfigurator configurator,
                               IEventService eventService,
                               ILogger logger)
            : base(resolver, configurator, eventService, logger)
        {
            Description = new PluginDescription
            {
                Name = "Wallpaper.Common",
                NeededServices = new[] { typeof(IMongoFactory) },
                ProvidedServices = new[] { typeof(IWallpaperManager) }
            };
        }

        protected override void ActivateInternal()
        {
            foreach (var registration in CreateRegistrations())
            {
                ConfigurationResolver.AddRegistration(registration);
            }
        }

        public IEnumerable<DependencyResolverRegistration> CreateRegistrations()
        {
            yield return new RepositoryRegistration<IWallpaperRepository, WallpaperMongoRepository, Model.Wallpaper>();
            yield return new RepositoryRegistration<IWallpaperDataRepository, WallpaperDataGridFSRepository, Model.WallpaperData>();
            yield return new RepositoryRegistration<IWallpaperDeployerConfigurationRepository, WallpaperDeployerConfigurationMongoRepository, WallpaperDeployerConfiguration>();
            yield return new RepositoryRegistration<IWallpaperActionRepository, WallpaperActionMongoRepository, WallpaperAction>();
            yield return new RepositoryRegistration<IClientRepository, ClientMongoRepository, Client>();
            yield return new RepositoryRegistration<IWorkItemRepository, WorkItemMongoRepository, WorkItem>();

            yield return new ManagerRegistration<IWallpaperManager, WallpaperManager, Model.Wallpaper>();
            yield return new ManagerRegistration<IWallpaperDeployerConfigurationManager, WallpaperDeployerConfigurationManager, WallpaperDeployerConfiguration>();
            yield return new ManagerRegistration<IWallpaperActionManager, WallpaperActionManager, WallpaperAction>();
            yield return new ManagerRegistration<IClientManager, ClientManager, Client>();
            yield return new ManagerRegistration<IWorkItemManager, WorkItemManager, WorkItem>();


        }
    }
}
