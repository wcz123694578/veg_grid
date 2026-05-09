using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VegGridLayouter.UI.ViewModels;

namespace VegGridLayouter.UI.Views
{
    /// <summary>
    /// AddRowPopupControl.xaml 的交互逻辑
    /// </summary>
    public partial class AddRowPopupControl : UserControl
    {
        public AddRowPopupControl()
        {
            InitializeComponent();

            bool isInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

            if (isInDesignMode)
            {
                return;
            }

            this.DataContext = ServiceLocator.Container.Resolve<AddRowPopupControlViewModel>();
        }
    }
}
