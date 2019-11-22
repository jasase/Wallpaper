using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Managers;
using Plugin.Application.Wallpaper.Common.Model;

namespace Plugin.Application.Wallpaper.Activities.WallpaperLoader.Spotlight
{
    public class SpotlightImageLoader : IWallpaperLoader<ItemResult>
    {
        private const string DEFAULT_URI = @"https://arc.msn.com/v3/Delivery/Cache?pid=209567&fmt=json&rafb=0&ua=WindowsShellClient%2F0&disphorzres=9999&dispvertres=9999&lo=80217&pl=de-DE&lc=de-DE&ctry=de";
        private readonly IWorkItemManager _workItemManager;

        public SpotlightImageLoader(IWorkItemManager workItemManager)
        {
            Source = new WallpaperSource
            {
                Name = "Spotlight",
                BaseUri = new Uri("http://arc.msn.com/")
            };
            _workItemManager = workItemManager;
        }

        public WallpaperSource Source { get; }

        public IEnumerable<ItemResult> LoadAvailiableImages()
        {
            var httpClient = new HttpClient();

            for (var i = 0; i < 250; i++)
            {
                var webResult = httpClient.GetAsync(DEFAULT_URI).Result;
                if (webResult.IsSuccessStatusCode)
                {
                    var resultContent = webResult.Content.ReadAsStringAsync().Result;
                    var listResult = JsonConvert.DeserializeObject<ListRequest>(resultContent);

                    if (listResult.batchrsp != null &&
                        listResult.batchrsp.items != null)
                    {

                        foreach (var item in listResult.batchrsp.items)
                        {
                            var imageResult = JsonConvert.DeserializeObject<ItemResult>(item.item);
                            if (imageResult.ad.title_text == null)
                            {
                                continue;
                            }

                            yield return imageResult;
                        }
                    }
                }
                else
                {
                    //TODO Logger
                }
            }
        }

        public WallpaperLoadResult LoadWallpaper(ItemResult wallpaper)
        {
            var uri = new Uri(wallpaper.ad.image_fullscreen_001_landscape.u);
            var httpClient = new HttpClient();
            var webResult = httpClient.GetAsync(uri).Result;

            return new WallpaperLoadResult(new WallpaperInformation
            {
                Caption = wallpaper.ad.title_text.tx,
                Hash = wallpaper.GetImageHash(),
                Url = uri,
                Source = Source
            },
            _workItemManager.ConvertToWorkItem(wallpaper),
            new[]
            {
                new WallpaperFileWithData
                {
                    FileDto = new WallpaperFileImage(),
                    Data = webResult.Content.ReadAsByteArrayAsync().Result,
                }
            });
        }
    }
}
