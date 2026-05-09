using Prism.DryIoc;
using Prism.Ioc;
using ScriptPortal.Vegas;
using System.Windows;
using VegGridLayouter.UI.ViewModels;
using VegGridLayouter.UI.Views;

namespace VegGridLayouter.UI
{
    public class Bootstrapper : PrismBootstrapper
    {
        private readonly Vegas _vegas;

        public Bootstrapper(Vegas vegas)
        {
            this._vegas = vegas;
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<Vegas>(() => _vegas);
            containerRegistry.Register<MainWindowViewModel>();

            containerRegistry.RegisterForNavigation<AddRowPopupControl, AddRowPopupControlViewModel>();
            containerRegistry.RegisterForNavigation<SetRowColumnPopup, SetRowColumnPopupViewModel>();

            containerRegistry.RegisterDialog<AboutWindow, AboutWindowViewModel>("About");
            ServiceLocator.Container = Container;
        }
    }

    public static class ServiceLocator
    {
        public static IContainerProvider Container { get; set; }
    }
}
