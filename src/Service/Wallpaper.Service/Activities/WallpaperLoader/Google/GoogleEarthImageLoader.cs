using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Managers;
using Plugin.Application.Wallpaper.Common.Model;

namespace Plugin.Application.Wallpaper.Activities.WallpaperLoader.Google
{
    public class GoogleEarthImageLoader : IWallpaperLoader<GoogleEarthImageId>
    {
        private readonly HttpClient _httpClient;
        private readonly IWorkItemManager _workItemManager;

        public WallpaperSource Source { get; }

        public GoogleEarthImageLoader(IWorkItemManager workItemManager)
        {
            Source = new WallpaperSource
            {
                Name = "Google-EarthView",
                BaseUri = new Uri("https://www.gstatic.com/prettyearth/assets/data/v3/")
            };
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = Source.BaseUri;
            _workItemManager = workItemManager;
        }

        public IEnumerable<GoogleEarthImageId> LoadAvailiableImages()
            => ImageIds.Ids.Select(x => new GoogleEarthImageId(x.ToString()));

        public WallpaperLoadResult LoadWallpaper(GoogleEarthImageId wallpaper)
        {
            var relativeUri = $"{wallpaper.Id}.json";
            var webResult = _httpClient.GetAsync(relativeUri).Result;

            var strResult = webResult.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<GoogleResult>(strResult);

            var pic = Convert.FromBase64String(result.dataUri.Split(',')[1]);
            result.dataUri = string.Empty; //Remove Data before convert to RawDocument

            var address = CreateAddress(result);
            return new WallpaperLoadResult(
                new WallpaperInformation
                {
                    Hash = wallpaper.Id,
                    Url = new Uri(Source.BaseUri, relativeUri),
                    Caption = address.Country + " " + string.Join(" ", address.AdministrativeArea) + " " + address.City + " " + string.Join(" ", address.SubCity) + " " + address.Street,
                    Locations = new List<WallpaperLocation>
                    {
                        new WallpaperLocationCoordinates
                        {
                            Latitude = result.lat,
                            Longtitude = result.lng
                        },
                        address
                    }
                },
                _workItemManager.ConvertToWorkItem(result),
                new WallpaperFileWithData[]
                {
                    new WallpaperFileWithData
                    {
                        Data = pic,
                        FileDto = new WallpaperFileImage()
                    }
                });
        }

        private WallpaperLocationAddress CreateAddress(GoogleResult result)
        {
            var adminArea = new string[]
            {
                result.geocode.administrative_area_level_1,
                result.geocode.administrative_area_level_2,
                result.geocode.administrative_area_level_3,
            };
            var subcity = new string[]
            {
                result.geocode.sublocality_level_1,
                result.geocode.sublocality_level_2,
                result.geocode.sublocality_level_3
            };

            return new WallpaperLocationAddress
            {
                Country = result.geocode.country,
                AdministrativeArea = adminArea.Where(x => !string.IsNullOrEmpty(x)).ToArray(),
                City = result.geocode.locality,
                SubCity = subcity.Where(x => !string.IsNullOrEmpty(x)).ToArray(),
                Street = result.geocode.route
            };
        }
    }
}
