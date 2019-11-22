//using System;
//using System.Net.Http;
//using System.Text;
//using Microsoft.OneDrive.Sdk;
//using Plugin.Application.Wallpaper.Common.Azure;
//using Plugin.Application.Wallpaper.Common.Azure.Authentication;
//using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Managers;
//using Plugin.Application.Wallpaper.Common.Model.Configurations;
//using Plugin.Application.Wallpaper.DataAccess.Contracts.Managers;

//namespace Plugin.Application.Wallpaper.Activities.WallpaperDeployers
//{
//    public class OneDriveDeployer : IWallpaperDeployer, IRefreshTokenStore
//    {
//        private readonly OneDriveDeployerConfiguration _configuration;
//        private readonly IWallpaperManager _wallpaperManager;
//        private readonly IWallpaperDeployerConfigurationManager _wallpaperDeployerConfigurationManager;
//        private readonly AzureAdAuthenticationService _authService;

//        public OneDriveDeployer(OneDriveDeployerConfiguration configuration,
//                                IWallpaperManager wallpaperManager,
//                                IWallpaperDeployerConfigurationManager wallpaperDeployerConfigurationManager)
//        {
//            _configuration = configuration;
//            _wallpaperManager = wallpaperManager;
//            _wallpaperDeployerConfigurationManager = wallpaperDeployerConfigurationManager;
//            _authService = new AzureAdAuthenticationService(this, AzureAdConfig.CLIENT_ID, AzureAdConfig.CLIENT_SECRET);
//            _authService.InitWithRefreshToken(_configuration.AzureOneDriveRefreshToken);
//        }

//        public void Deploy(Common.Model.Wallpaper wallpaper)
//        {
//            var client = new OneDriveClient(_authService);



//            var item = client.Drive.Items["bilder/wallpaper"].Request().GetAsync().Result;            

//            var drive = client.Drive.Request().GetAsync().Result;

//        }

//        public void StoreRefreshToken(string newRefreshToken)
//        {
//            _configuration.AzureOneDriveRefreshToken = newRefreshToken;
//            _wallpaperDeployerConfigurationManager.UpdateRefreshToken(_configuration);
//        }
//    }
//}
