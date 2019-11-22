using Framework.Contracts.Entities;

namespace Plugin.Application.Wallpaper.Common.Model.WorkItems
{
    public abstract class WorkItemKompositum : Entity
    {
        public string Name { get; }

        public WorkItemKompositum(string name)
        {
            Name = name.ToLowerInvariant();
        }

        public abstract void Accept(IWorkItemVisitor visitor);

        public abstract TReturn Accept<TReturn>(IWorkItemVisitor<TReturn> visitor);
    }
}
