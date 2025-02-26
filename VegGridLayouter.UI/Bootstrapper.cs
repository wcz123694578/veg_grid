using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using System.Windows;
using VegGridLayouter.UI.ViewModels;
using VegGridLayouter.UI.Views;

namespace VegGridLayouter.UI
{
    public class Bootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.Register<MainWindow, MainWindowViewModel>();

            ViewModelLocationProvider.Register<SetRowColumnPopup, SetRowColumnPopupViewModel>();
        }
    }
}
