using Prism.Ioc;
using System.Windows.Controls;
using VegGridLayouter.UI.ViewModels;

namespace VegGridLayouter.UI.Views
{
    /// <summary>
    /// PropertiesControl.xaml 的交互逻辑
    /// </summary>
    public partial class PropertiesControl : UserControl
    {
        public PropertiesControl()
        {
            InitializeComponent();

            this.DataContext = ServiceLocator.Container.Resolve<PropertiesControlViewModel>();
        }
    }
}
