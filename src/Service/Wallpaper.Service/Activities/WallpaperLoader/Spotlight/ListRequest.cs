using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.Application.Wallpaper.Activities.WallpaperLoader.Spotlight
{
#pragma warning disable IDE1006 // Naming Styles
    public class ListRequest
    {
        public ListRequestBatchResponse batchrsp { get; set; }
    }

    public class ListRequestBatchResponse
    {
        public string ver { get; set; }
        public ListRequestItem[] items { get; set; }
        public string refreshtime { get; set; }
    }

    public class ListRequestItem
    {
        public string item { get; set; }
    }
#pragma warning restore IDE1006 // Naming Styles
}
