using Framework.Contracts.Services.DataAccess;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Repositories;
using Plugin.Application.Wallpaper.Common.Model.WorkItems;
using PluginMongoDb;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Implementation.Repositories
{
    public class WorkItemMongoRepository
        : EntityMongoRepository<WorkItem>,
          IWorkItemRepository
    {
        public WorkItemMongoRepository(IMongoDataAccessProvider dataAccessProvider)
            : base(dataAccessProvider)
        { }
    }
}
