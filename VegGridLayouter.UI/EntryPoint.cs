using Prism.Events;
using ScriptPortal.Vegas;
using System.Windows.Threading;
using VegGridLayouter.Core;

namespace VegGridLayouter.UI
{
    public class EntryPoint
    {
        public void FromVegas(Vegas vegas)
        {
            //var MainWindow = new MainWindow();
            //MainWindow.ShowDialog();

            StaticVariable.CurrentDispatcher = Dispatcher.CurrentDispatcher;

            VegasContextFactory.Initialize(vegas);

            Bootstrapper bootstrapper = new Bootstrapper(vegas);
            bootstrapper.Run();
        }
    }
}
