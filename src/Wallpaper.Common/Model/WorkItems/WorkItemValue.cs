using System;

namespace Plugin.Application.Wallpaper.Common.Model.WorkItems
{
    public class WorkItemValue : WorkItemCollectionElement
    {
        public object Value { get; }

        public WorkItemValue(string name, object value)
            : base(name)
        {
            var valueType = value.GetType();
            if (!valueType.IsPrimitive && !typeof(string).Equals(valueType) )
            {
                throw new InvalidOperationException("WorkItem only support value types");
            }
            Value = value;
        }

        public override void Accept(IWorkItemVisitor visitor)
            => visitor.Handle(this);

        public override TReturn Accept<TReturn>(IWorkItemVisitor<TReturn> visitor)
            => visitor.Handle(this);        
    }
}
