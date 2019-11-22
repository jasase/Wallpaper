using System.Linq;
using System.Text;

namespace Plugin.Application.Wallpaper.Activities.WallpaperLoader.Google
{
#pragma warning disable IDE1006 // Benennungsstile
    public class GoogleResult
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public int zoom { get; set; }
        public string user { get; set; }
        public string id { get; set; }
        public string attribution { get; set; }
        public Rating ratings { get; set; }
        public Geocode geocode { get; set; }
        public string dataUri { get; set; }
    }

    public class Rating
    {
        public int count { get; set; }
        public double average { get; set; }
        public bool rejected { get; set; }
    }
    public class Geocode
    {
        public string country { get; set; }
        public string administrative_area_level_1 { get; set; }
        public string administrative_area_level_2 { get; set; }
        public string administrative_area_level_3 { get; set; }
        public string locality { get; set; }
        public string sublocality_level_1 { get; set; }
        public string sublocality_level_2 { get; set; }
        public string sublocality_level_3 { get; set; }
        public string route { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            AppendIfNotEmpty(sb, country);
            AppendIfNotEmpty(sb, administrative_area_level_1);
            AppendIfNotEmpty(sb, administrative_area_level_2);
            AppendIfNotEmpty(sb, administrative_area_level_3);
            AppendIfNotEmpty(sb, locality);
            AppendIfNotEmpty(sb, sublocality_level_1);
            AppendIfNotEmpty(sb, sublocality_level_2);
            AppendIfNotEmpty(sb, sublocality_level_3);
            AppendIfNotEmpty(sb, route);

            var result = sb.ToString();
            if (result.Any())
            {
                return result;
            }

            return base.ToString();
        }

        private void AppendIfNotEmpty(StringBuilder sb, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                sb.Append(" ");
                sb.Append(value);
            }
        }
    }
#pragma warning restore IDE1006 // Benennungsstile
}
