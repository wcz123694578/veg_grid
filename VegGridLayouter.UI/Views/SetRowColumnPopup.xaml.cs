using Prism.Events;
using Prism.Ioc;
using System.ComponentModel;
using System.Windows;
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

            bool isInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

            if (isInDesignMode)
            {
                return;
            }

            var setRowColumnPopupViewModel = ServiceLocator.Container.Resolve<SetRowColumnPopupViewModel>();
            this.DataContext = setRowColumnPopupViewModel;
        }
    }
}
