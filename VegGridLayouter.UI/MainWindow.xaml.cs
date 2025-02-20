using System.IO;
using System.Windows;
using VegGridLayouter.Core;
using VegGridLayouter.Parser;

namespace VegGridLayouter.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //VegGrid grid = new VegGrid();
            //FileStream fs = File.Open("test.xml", FileMode.Open);
            //grid = VegXmlDeserializer.DeserializeXml(fs);

            //int width = grid.CurProject.Video.Width;
            //int height = grid.CurProject.Video.Height;

            //grid.CalculateLayout(width, height);

            //grid.Generate();
        }
    }
}
