using Prism.Events;
using System;

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

        public static string LogFileName = "layouter_log.txt";

        public static string ConfigFileName = "layouter_config.ini";

        public static string FilePath = $@"{Environment.GetEnvironmentVariable("AppData")}\Vegas Pro";
    }
}
