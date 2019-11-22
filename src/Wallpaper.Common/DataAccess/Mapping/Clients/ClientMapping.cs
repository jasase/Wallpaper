using MongoDB.Bson.Serialization;
using Plugin.Application.Wallpaper.Common.Model.Clients;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Mapping.Clients
{
    public class ClientMapping : BsonClassMap<Client>
    {
        public ClientMapping()
        {
            MapProperty(x => x.Created);
            MapProperty(x => x.Name);
            MapProperty(x => x.ExternalId);
        }
    }
}
