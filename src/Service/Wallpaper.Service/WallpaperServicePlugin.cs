using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Core.Scheduling;
using Framework.Abstraction.Extension;
using Framework.Abstraction.IocContainer;
using Framework.Abstraction.Plugins;
using Framework.Abstraction.Services;
using Framework.Abstraction.Services.Scheduling;
using Plugin.Application.Wallpaper.DataAccess.Contracts.Managers;
using Plugin.Application.Wallpaper.Activities.WallpaperLoader.Bing;
using Plugin.MongoDb.Interfaces;
using ServiceHost.Contracts;
using Plugin.Application.Wallpaper.Activities.WallpaperLoader;
using Plugin.Application.Wallpaper.Activities.WallpaperLoader.Google;
using System.Collections.Concurrent;
using Plugin.Application.Wallpaper.Activities.ImageConverter;
using Framework.Abstraction.Services.ThreadManaging;
using Framework.Abstraction.Messages.EntityMessages;
using Plugin.Application.Wallpaper.Common;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Managers;
using Plugin.Application.Wallpaper.Common.Model.Configurations;
using Plugin.Application.Wallpaper.Activities.WallpaperDeployers;
using Plugin.Application.Wallpaper.Activities.WallpaperLoader.Spotlight;

namespace Plugin.Application.Wallpaper
{
    public class WallpaperServicePlugin : Framework.Contracts.Plugins.Plugin, IServicePlugin
    {
        public override PluginDescription Description { get; }

        public WallpaperServicePlugin(IDependencyResolver resolver,
                               IDependencyResolverConfigurator configurator,
                               IEventService eventService,
                               ILogger logger)
            : base(resolver, configurator, eventService, logger)
        {
            Description = new AutostartServicePluginDescription
            {
                Name = "Wallpaper.Service",
                NeededServices = new[] { typeof(IConfiguration),
                                         typeof(ISchedulingService),
                                         typeof(IMongoFactory),
                                         typeof(IThreadManager),
                                         typeof(IWallpaperManager) }
            };
        }

        protected override void ActivateInternal()
        {
            SetupDeployer();
            SetupLoader();
            SetupConverter();
        }

        private IEnumerable<WallpaperLoaderHandler> CreateLoaders(IWallpaperManager manager, ILogManager logManager, IWorkItemManager workItemManager)
        {
            yield return new WallpaperLoaderHandler<BingImage>(new BingWallpaperLoader(workItemManager), manager, workItemManager, logManager.GetLogger<WallpaperLoaderHandler>());
            yield return new WallpaperLoaderHandler<GoogleEarthImageId>(new GoogleEarthImageLoader(workItemManager), manager, workItemManager, logManager.GetLogger<WallpaperLoaderHandler>());
            yield return new WallpaperLoaderHandler<ItemResult>(new SpotlightImageLoader(workItemManager), manager, workItemManager, logManager.GetLogger<WallpaperLoaderHandler>());
        }

        private void SetupDeployer()
        {
            var configManager = Resolver.GetInstance<IWallpaperDeployerConfigurationManager>();
            var manager = Resolver.GetInstance<IWallpaperManager>();

            //foreach (var config in configManager.GetAll().OfType<OneDriveDeployerConfiguration>().Where(x => x.Active))
            //{
            //    var deployer = new OneDriveDeployer(config, manager, configManager);
            //    deployer.Deploy(null);
            //}
        }

        private void SetupLoader()
        {
            var logManager = Resolver.GetInstance<ILogManager>();
            var manager = Resolver.GetInstance<IWallpaperManager>();
            var workItemManager = Resolver.GetInstance<IWorkItemManager>();

            var job = new WallpaperJob(CreateLoaders(manager, logManager, workItemManager).ToArray(), manager, workItemManager, logManager.GetLogger<WallpaperJob>());

            var scheduler = Resolver.GetInstance<ISchedulingService>();
            scheduler.AddJob(job, new PollingPlan(TimeSpan.FromHours(6)));
        }

        private void SetupConverter()
        {
            var logManager = Resolver.GetInstance<ILogManager>();
            var manager = Resolver.GetInstance<IWallpaperManager>();
            var threadManager = Resolver.GetInstance<IThreadManager>();

            var collection = new BlockingCollection<ImageConverterQueueElement>();
            var thread = new ImageConverterThread(collection, logManager.GetLogger<ImageConverterThread>(), manager);
            threadManager.Start(thread);

            EventService.Register<EntityInsertedMessage>(x =>
            {
                if (x.EntityType == typeof(Common.Model.Wallpaper))
                {
                    collection.Add(new ImageConverterQueueElement
                    {
                        WallpaperId = x.EntityId
                    });
                }
            });
        }
    }
}
