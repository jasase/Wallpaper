using System;
using System.Collections;
using System.Linq;
using Framework.Common.Services.DataAccess;
using Framework.Common.Services.DataAccess.EntityDescriptions;
using Framework.Contracts.Extension;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Managers;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Repositories;
using Plugin.Application.Wallpaper.Common.Model.WorkItems;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Implementation.Managers
{
    public class WorkItemManager : EntityManager<WorkItem>, IWorkItemManager
    {
        public WorkItemManager(IWorkItemRepository repository,
                               IEventService eventService)
            : base(repository, eventService)
        { }

        protected override EntityDescription<WorkItem> CreateEntityDescription()
            => new EntityDescription<WorkItem>("WorkItem",
                                             new PropertyGroupDescription<WorkItem>[0],
                                             new PropertyDescription<WorkItem>[0]);


        public WorkItem ConvertToWorkItem<TValue>(TValue value) where TValue : class
        {
            var result = new WorkItem();
            ConvertToWorkItem(result, value);

            return result;
        }

        private void ConvertToWorkItem(WorkItem item, object value)
        {
            if (value == null) return;

            var valueType = value.GetType();
            var properties = valueType.GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string))
                {
                    item.Set(property.Name, property.GetValue(value));
                }
                else if (property.PropertyType.GetInterface(nameof(IEnumerable)) != null)
                {
                    var enumerable = property.GetValue(value) as IEnumerable;
                    var itemEnumerable = enumerable.OfType<object>().Select(x => ConvertToWorkItem(x));

                    item.Set(new WorkItemCollection(property.Name, itemEnumerable));
                }
                else
                {
                    var childItem = new WorkItem(property.Name, new WorkItemKompositum[0]);
                    var childValue = property.GetValue(value);
                    ConvertToWorkItem(childItem, childValue);

                    item.Set(childItem);
                }
            }
        }
    }
}
