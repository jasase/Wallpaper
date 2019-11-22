using Framework.Contracts.Services.DataAccess;
using Plugin.Application.Wallpaper.Common.Model.WorkItems;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Repositories
{
    public interface IWorkItemRepository
        : IRepository<WorkItem>
    { }
}
