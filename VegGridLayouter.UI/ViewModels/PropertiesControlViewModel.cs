using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using VegGridLayouter.UI.Events;

namespace VegGridLayouter.UI.ViewModels
{
    public class PropertiesControlViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        public ObservableCollection<PropertyItemViewModel> Properties { get; set; } = new ObservableCollection<PropertyItemViewModel>();

        public PropertiesControlViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<PropertiesLoadEvent>().Subscribe(OnPropertiesLoad);
        }

        private void OnPropertiesLoad(PropertiesLoadEventModel model)
        {
            Properties.Clear();

            if (StaticVariable.CollectionDictionary.Contains(model.Item.Name)) return;

            var attributes = model.Item.Attributes;
            var attributeProperties = model.Item.Attributes.Select(a => new PropertyItemViewModel
            {
                Name = a.Type,
                Value = a.Value,
                DisplayName = a.Type
            });

            var elementProperties = model.Item.Children.Select(c => new PropertyItemViewModel
            {
                Name = c.Name,
                Value = c.Value,
                DisplayName = c.Header
            });

            Properties.AddRange(attributeProperties);
            Properties.AddRange(elementProperties);
        }
    }

    public class PropertyItemViewModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public object Value { get; set; }
    }
}
