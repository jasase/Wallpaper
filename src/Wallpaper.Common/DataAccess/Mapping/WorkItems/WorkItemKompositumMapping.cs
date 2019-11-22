using MongoDB.Bson.Serialization;
using Plugin.Application.Wallpaper.Common.Model.WorkItems;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Mapping.WorkItems
{
    public class WorkItemKompositumMapping : BsonClassMap<WorkItemKompositum>
    {
        public WorkItemKompositumMapping()
        {
            MapProperty(x => x.Name);
        }
    }
}
