using Prism.Events;
using System;
using System.Threading;
using System.Windows.Threading;

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

        public static Dispatcher CurrentDispatcher;

        public static string[] CollectionDictionary
        {
            get
            {
                return new string[]
                {
                    "Children", "RowDefinitions", "ColumnDefinitions"
                };
            }
        }
    }
}
