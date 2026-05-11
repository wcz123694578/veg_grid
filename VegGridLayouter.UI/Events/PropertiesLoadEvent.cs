using Prism.Events;
using VegGridLayouter.UI.ViewModels;

namespace VegGridLayouter.UI.Events
{
    public class PropertiesLoadEvent : PubSubEvent<PropertiesLoadEventModel>
    {
    }

    public record PropertiesLoadEventModel
    {
        public TreeViewItemViewModel Item { get; set; }
    }
}
