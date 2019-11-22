using Framework.Abstraction.Services.DataAccess;
using Plugin.Application.Wallpaper.Common.Model.Clients;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Repositories
{
    public interface IClientRepository
        : IRepository<Client>
    {
        Client GetExternalClient(string externalId, string currentUsername);
    }
}
