using MongoDB.Bson.Serialization;
using Plugin.Application.Wallpaper.Common.Model.WorkItems;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Mapping.WorkItems
{
    public class WorkItemValueMapping : BsonClassMap<WorkItemValue>
    {
        public WorkItemValueMapping()
        {
            MapProperty(x => x.Value);
            MapCreator(x => new WorkItemValue(x.Name, x.Value));
            SetDiscriminator("Value");
        }
    }
}
