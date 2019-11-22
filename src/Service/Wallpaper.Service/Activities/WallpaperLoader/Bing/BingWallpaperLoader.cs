using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Managers;
using Plugin.Application.Wallpaper.Common.Model;
using Plugin.Application.Wallpaper.Common.Model.WorkItems;

namespace Plugin.Application.Wallpaper.Activities.WallpaperLoader.Bing
{
    public class BingWallpaperLoader : IWallpaperLoader<BingImage>
    {
        public const string URI = "http://www.bing.com/HPImageArchive.aspx?format=js&idx={0}&n=1";
        private const string START_OF_COPYRIGHT = "(©";

        private readonly IWorkItemManager _workItemManager;

        public WallpaperSource Source { get; }

        public BingWallpaperLoader(IWorkItemManager workItemManager)
        {
            _workItemManager = workItemManager;
            Source = new WallpaperSource
            {
                Name = "Bing",
                BaseUri = new Uri(URI)
            };
        }

        public IEnumerable<BingImage> LoadAvailiableImages()
        {
            var httpClient = new HttpClient();
            for (var i = 0; i < 20; i++)
            {
                var webResult = httpClient.GetAsync(string.Format(URI, i)).Result;
                if (webResult.IsSuccessStatusCode)
                {
                    var resultContent = webResult.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<BingResult>(resultContent);

                    if (result != null)
                    {
                        foreach (var image in result.images)
                        {
                            yield return image;
                        }
                    }
                }
            }
        }

        public WallpaperLoadResult LoadWallpaper(BingImage wallpaper)
        {
            var index = wallpaper.copyright.IndexOf(START_OF_COPYRIGHT);
            var caption = wallpaper.copyright.Substring(0, index).Trim();

            var httpClient = new HttpClient();
            var uri = new Uri(new Uri("http://www.bing.com"), wallpaper.url);
            var webResult = httpClient.GetAsync(uri).Result;

            return new WallpaperLoadResult(new WallpaperInformation
            {
                Caption = caption,
                Hash = wallpaper.hsh,
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
