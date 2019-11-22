using System.Collections.Generic;
using System.Linq;

namespace Plugin.Application.Wallpaper.Common.Model.WorkItems
{
    public class WorkItemCollection : WorkItemKompositum
    {
        private readonly WorkItemCollectionElement[] _elements;

        public IEnumerable<WorkItemCollectionElement> Elements => _elements;

        public WorkItemCollection(string name, IEnumerable<WorkItemCollectionElement> elements)
            : base(name)
        {
            _elements = elements.ToArray();
        }


        public override void Accept(IWorkItemVisitor visitor)
            => visitor.Handle(this);

        public override TReturn Accept<TReturn>(IWorkItemVisitor<TReturn> visitor)
            => visitor.Handle(this);
    }
}
