using ScriptPortal.Vegas;
using System;
using System.Threading;
using System.Windows;
using VegGridLayouter.Core;

namespace VegGridLayouter.UI
{
    public class EntryPoint
    {
        // [STAThread]
        public void FromVegas(Vegas vegas)
        {
            VegasManager.Instance = vegas;

            MainWindow mainWindow = new MainWindow();
            mainWindow.ShowDialog();
        }
    }
}
