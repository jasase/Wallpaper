using System;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Managers
{
    public interface IClientManagerCurrentClientHandler
    {
        Guid GetCurrentClientId();
    }
}
