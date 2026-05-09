using Prism.Events;
using Prism.Ioc;
using System.Windows.Controls;
using VegGridLayouter.UI.ViewModels;

namespace VegGridLayouter.UI.Views
{
    /// <summary>
    /// AddChildPopup.xaml 的交互逻辑
    /// </summary>
    public partial class SetRowColumnPopup : UserControl
    {
        public SetRowColumnPopup()
        {
            InitializeComponent();

            var setRowColumnPopupViewModel = ServiceLocator.Container.Resolve<SetRowColumnPopupViewModel>();
            this.DataContext = setRowColumnPopupViewModel;
        }
    }
}
