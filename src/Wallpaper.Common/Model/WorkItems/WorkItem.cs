using System.Collections.Generic;
using System.Linq;

namespace Plugin.Application.Wallpaper.Common.Model.WorkItems
{
    public class WorkItem : WorkItemCollectionElement
    {
        private const string ROOT_NAME = "root";

        private Dictionary<string, WorkItemKompositum> _children;

        public IEnumerable<WorkItemKompositum> Children => _children.Values;

        public WorkItem()
            : this(ROOT_NAME, new WorkItemKompositum[0])
        { }

        public WorkItem(string name, IEnumerable<WorkItemKompositum> children)
            : base(name)
        {
            _children = children.ToDictionary(x => x.Name);
        }

        public override void Accept(IWorkItemVisitor visitor)
            => visitor.Handle(this);

        public override TReturn Accept<TReturn>(IWorkItemVisitor<TReturn> visitor)
            => visitor.Handle(this);

        public void Set(string name, object v)
        {
            if(v == null)
            {
                return;
            }
            if (v is WorkItemCollection)
            {
                Set(v as WorkItemCollection);
            }
            else if (v is WorkItem)
            {
                Set(v as WorkItem);
            }
            else if (v.GetType().IsPrimitive)
            {
                _children[name] = new WorkItemValue(name, v);
            }
            else
            {
                _children[name] = new WorkItemValue(name, v.ToString());
            }
        }

        public void Set(WorkItemCollection workItemCollection)
            => _children[workItemCollection.Name] = workItemCollection;

        public void Set(WorkItem childItem)
            => _children[childItem.Name] = childItem;
    }
}
