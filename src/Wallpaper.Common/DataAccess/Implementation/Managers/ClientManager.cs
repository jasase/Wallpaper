using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Core.Helper;
using Framework.Core.Services.DataAccess;
using Framework.Core.Services.DataAccess.EntityDescriptions;
using Framework.Abstraction.Extension;
using Framework.Abstraction.Helper;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Managers;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Repositories;
using Plugin.Application.Wallpaper.Common.Model.Clients;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Implementation.Managers
{
    public class ClientManager : EntityManager<Client>, IClientManager
    {
        private readonly IClientRepository _repository;
        private readonly List<IClientManagerCurrentClientHandler> _currentClientHandler;

        public ClientManager(IClientRepository repository, IEventService eventService)
            : base(repository, eventService)
        {
            _repository = repository;
            _currentClientHandler = new List<IClientManagerCurrentClientHandler>();
        }

        public IMaybe<Client> GetCurrentClient()
        {
            foreach (var handler in _currentClientHandler)
            {
                var curId = handler.GetCurrentClientId();
                if (curId != Guid.Empty)
                {
                    return _repository.GetById(curId);
                }
            }

            return new Maybe<Client>(null);
        }

        public Task<Client> GetExternalClient(string externalId, string currentUsername)
            => Task<Client>.Factory.StartNew(() => _repository.GetExternalClient(externalId, currentUsername));

        public void RegisterCurrentClientHandler(IClientManagerCurrentClientHandler handler)
            => _currentClientHandler.Add(handler);

        protected override EntityDescription<Client> CreateEntityDescription()
            => new EntityDescription<Client>("Client",
                                             new PropertyGroupDescription<Client>[0],
                                             new PropertyDescription<Client>[0]);
    }
}
