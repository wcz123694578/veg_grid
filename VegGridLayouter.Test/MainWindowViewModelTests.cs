using Moq;
using Prism.Events;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using VegGridLayouter.UI.ViewModels;
using Xunit;

namespace VegGridLayouter.Test
{
    public class MainWindowViewModelTests
    {
        public MainWindowViewModelTests()
        {
            
        }

        [Fact]
        public void ParseXml_ShouldParseValidXml()
        {
            var viewModel = new MainWindowViewModel(new EventAggregator(), new Mock<IDialogService>().Object);

            var xml = @"
<VegGrid>
    <RowDefinitions>
        <RowDefinition Type=""Star"" Value=""1"" />
        <RowDefinition />
    </RowDefinitions>
    <ColumnDefinitions>
        <ColumnDefinition Type=""Star"" Value=""1"" />
    </ColumnDefinitions>
    <Children>
        <VegGrid/>
    </Children>
</VegGrid>
";

            // Act
            var rootElement = System.Xml.Linq.XElement.Parse(xml);
            ObservableCollection<TreeViewItemViewModel> treeViewItemViewModels = viewModel.ParseXml(rootElement);
        }
    }
}
