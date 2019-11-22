using MongoDB.Bson.Serialization;
using Plugin.Application.Wallpaper.Common.Model.WorkItems;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Mapping.WorkItems
{
    public class WorkItemMapping : BsonClassMap<WorkItem>
    {
        public WorkItemMapping()
        {
            MapProperty(x => x.Children);
            MapCreator(x => new WorkItem(x.Name, x.Children));
            SetDiscriminator("Item");
        }
    }
}
