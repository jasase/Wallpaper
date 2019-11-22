using Framework.Abstraction.Services.DataAccess;
using Framework.Contracts.Services.DataAccess;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Repositories;
using Plugin.Application.Wallpaper.Common.Model.WorkItems;
using Plugin.DataAccess.MongoDb;

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
