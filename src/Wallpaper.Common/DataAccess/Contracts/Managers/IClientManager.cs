using System.Threading.Tasks;
using Framework.Contracts.Helper;
using Framework.Contracts.Services.DataAccess;
using Plugin.Application.Wallpaper.Common.Model.Clients;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Managers
{
    public interface IClientManager
        : IManager<Client>
    {
        Task<Client> GetExternalClient(string externalId, string currentUsername);
        IMaybe<Client> GetCurrentClient();

        void RegisterCurrentClientHandler(IClientManagerCurrentClientHandler handler);
    }
}
