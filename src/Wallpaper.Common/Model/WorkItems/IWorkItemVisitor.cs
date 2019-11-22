namespace Plugin.Application.Wallpaper.Common.Model.WorkItems
{
    public interface IWorkItemVisitor
    {
        void Handle(WorkItem workItem);
        void Handle(WorkItemCollection workItem);
        void Handle(WorkItemValue workItem);
    }

    public interface IWorkItemVisitor<TReturn>
    {
        TReturn Handle(WorkItem workItem);
        TReturn Handle(WorkItemCollection workItem);
        TReturn Handle(WorkItemValue workItem);
    }
}
