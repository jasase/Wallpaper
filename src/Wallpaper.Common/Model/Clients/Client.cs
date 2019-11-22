using System;
using Framework.Abstraction.Entities;

namespace Plugin.Application.Wallpaper.Common.Model.Clients
{
    public class Client : Entity
    {
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
    }
}
