using System;
using System.Windows;
using System.Windows.Controls;
using AutoUpdate.Abstraction;
using Framework.Abstraction.Extension;
using Framework.Abstraction.IocContainer;
using Framework.Abstraction.IocContainer.Registrations;
using Framework.Abstraction.Plugins;
using Framework.Abstraction.Services.ThreadManaging;
using Hardcodet.Wpf.TaskbarNotification;
using WpfUi.Contracts.Plugins;
using WpfUi.Contracts.Services;
using WpfUi.Contracts.Services.UiModuls;

namespace Plugin.Application.Wallpaper.Client
{
    public class WallpaperClientPlugin : Framework.Contracts.Plugins.Plugin, IUiPlugin
    {
        public WallpaperClientPlugin(IDependencyResolver resolver, IDependencyResolverConfigurator configurator, IEventService eventService, ILogger logger)
            : base(resolver, configurator, eventService, logger)
        {
            //Unosquare.FFME.MediaElement.FFmpegDirectory = GetType().Assembly.Location;

            Description = new PluginDescription
            {
                Name = "Wallpaper client",
                Description = "Wallpaper client",
                NeededServices = new[] { typeof(IUiApplication),
                                         typeof(IResourceDictionaryManager),
                                         typeof(IThreadManager)}
            };
        }

        public override PluginDescription Description { get; }

        protected override void ActivateInternal()
        {
            var versionService = new WallpaperClientVersionService();
            var registration = new SingletonRegistration<WallpaperClientVersionService>(versionService);
            ConfigurationResolver.AddRegistration(registration);

            var uiApp = Resolver.GetInstance<IUiApplication>();
            uiApp.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var resourceDictionaryManager = Resolver.GetInstance<IResourceDictionaryManager>();
            var dir = new ResourceDictionary { Source = new Uri("/Plugin.Application.Wallpaper.Client;component/WallpaperClientResourceDictionary.xaml", UriKind.RelativeOrAbsolute) };
            resourceDictionaryManager.AddToApplication(dir);

            var result = dir["WallpaperNotifyIcon"] as TaskbarIcon;
            if (result != null)
            {
                var viewModel = Resolver.CreateConcreteInstanceWithDependencies<TaskBarIconViewModel>();

                ConfigurationResolver.AddRegistration(new ServiceInstanceRegistration<IUserInteraction>(viewModel));

                result.DataContext = viewModel;
                result.Loaded += (s, e) => viewModel.State.OnLogin();
            }

            //Workaround for bug in NotfiyIcon library
            var tt = new ToolTip();
            tt.IsOpen = true;
            tt.IsOpen = false;
        }
    }
}
