using MongoDB.Bson.Serialization;
using Plugin.Application.Wallpaper.Common.Model.WorkItems;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Mapping.WorkItems
{
    public class WorkItemCollectionMapping : BsonClassMap<WorkItemCollection>
    {
        public WorkItemCollectionMapping()
        {
            MapProperty(x => x.Elements);
            MapCreator(x => new WorkItemCollection(x.Name, x.Elements));
            SetDiscriminator("Collection");
        }
    }
}
