using Framework.Contracts.Services.DataAccess;
using Plugin.Application.Wallpaper.Common.Model.WorkItems;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Managers
{
    public interface IWorkItemManager
        : IManager<WorkItem>
    {
        WorkItem ConvertToWorkItem<TValue>(TValue value)
            where TValue : class;
    }
}
