using ICSharpCode.AvalonEdit.Folding;
using Prism.Ioc;
using System;
using System.Windows;
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

        private readonly IContainerProvider _container;

        public MainWindow()
        {
            
        }

        public MainWindow(IContainerProvider container)
        {
            _container = container;
            InitializeComponent();

            MainWindowViewModel mainWindowViewModel = _container.Resolve<MainWindowViewModel>();
            this.DataContext = mainWindowViewModel;

            //WindowsManager.Register<AboutWindow>("AboutWindow");

            ICSharpCode.AvalonEdit.Search.SearchPanel.Install(TextEditor);
            foldingManager = FoldingManager.Install(TextEditor.TextArea);

            TextEditor.TextArea.Caret.PositionChanged += TextEditor_TextArea_Caret_PositionChanged;
        }

        private void TextEditor_TextArea_Caret_PositionChanged(object sender, EventArgs e)
        {

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
