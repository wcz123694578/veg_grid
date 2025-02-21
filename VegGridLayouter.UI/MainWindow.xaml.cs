using ICSharpCode.AvalonEdit.Folding;
using System.IO;
using System.Windows;
using VegGridLayouter.Core;
using VegGridLayouter.Parser;
using VegGridLayouter.UI.ViewModels;

namespace VegGridLayouter.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private FoldingManager foldingManager = null;
        XmlFoldingStrategy foldingStrategy = new XmlFoldingStrategy();

        public MainWindow()
        {
            InitializeComponent();
            ICSharpCode.AvalonEdit.Search.SearchPanel.Install(TextEditor);
            foldingManager = FoldingManager.Install(TextEditor.TextArea);
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

        private void TextEditor_TextChanged(object sender, System.EventArgs e)
        {
            if (foldingManager == null) return;
            foldingStrategy.UpdateFoldings(foldingManager, TextEditor.Document);
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (foldingManager == null) return;
            var isFrist = true;
            foreach (var item in foldingManager.AllFoldings)
            {
                if (isFrist)
                {
                    isFrist = false;
                    continue;
                }
                item.IsFolded = true;
            }
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (foldingManager == null) return;
            foreach (var item in foldingManager.AllFoldings)
            {
                item.IsFolded = false;
            }
        }
    }
}
