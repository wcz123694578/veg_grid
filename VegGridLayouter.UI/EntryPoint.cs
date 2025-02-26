using Prism.Events;
using ScriptPortal.Vegas;
using Unity;
using VegGridLayouter.Core;

namespace VegGridLayouter.UI
{
    public class EntryPoint
    {
        public void FromVegas(Vegas vegas)
        {
            VegasManager.Instance = vegas;

            StaticVariable.eventAggregator = new EventAggregator();

            var MainWindow = new MainWindow();
            MainWindow.ShowDialog();
        }
    }
}
