using Prism.Events;

namespace VegGridLayouter.UI
{
    public static class StaticVariable
    {
        public static IEventAggregator eventAggregator;

        public enum TreeViewStateType
        {
            Lock_,
            Unlock_
        };

        public static TreeViewStateType TreeViewState { get; set; } = TreeViewStateType.Unlock_;
    }
}
