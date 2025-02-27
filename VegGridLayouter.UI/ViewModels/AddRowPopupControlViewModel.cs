using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using VegGridLayouter.UI.Events;

namespace VegGridLayouter.UI.ViewModels
{
    public class AddRowPopupControlViewModel : BindableBase
    {
        public string Type { get; set; }

        private string _value;

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged(nameof(Value));
            }
        }

        public List<string> TypeList { get; } = new List<string>() { "Star", "Fixed" };

        private int _typeListSelectedIndex = 0;

        public int TypeListSelectedIndex
        {
            get { return _typeListSelectedIndex; }
            set
            {
                _typeListSelectedIndex = value;
                RaisePropertyChanged(nameof(TypeListSelectedIndex));
            }
        }

        private int _addCount = 1;

        public int AddCount
        {
            get { return _addCount; }
            set
            {
                _addCount = value;
                RaisePropertyChanged(nameof(AddCount));
            }
        }



        public TreeViewItemViewModel SelectedItem { get; set; }

        private IEventAggregator _aggregator = StaticVariable.eventAggregator;

        public AddRowPopupControlViewModel()
        {
            ConfirmAddRowCommand = new DelegateCommand(confirmAddRowFunc);
            _aggregator.GetEvent<SelectTreeViewItemEvent>().Subscribe(SelectTreeViewItemEventProcesser);
        }

        private void SelectTreeViewItemEventProcesser(SelectTreeViewItemEventModel model)
        {
            this.SelectedItem = model.SelectedItem;
        }

        public DelegateCommand ConfirmAddRowCommand { get; set; }

        private void confirmAddRowFunc()
        {

            string listName = this.SelectedItem.Name.Substring(0, this.SelectedItem.Name.Length - 1);

            this.Type = TypeList[TypeListSelectedIndex];

            for (int i = 0; i < this.AddCount; i++)
            {
                this.SelectedItem.Children.Add(new TreeViewItemViewModel(null, _aggregator) {
                    Header = "行",
                    Name = listName,
                    Attributes = new ObservableCollection<AttributeItem>()
                    {
                        new AttributeItem("Type", this.Type),
                        new AttributeItem("Value", this.Value)
                    },
                    Parent = SelectedItem,
                    IsChild = true
                });
            }
            StaticVariable.TreeViewState = StaticVariable.TreeViewStateType.Lock_;
            _aggregator.GetEvent<UpdateXmlEvent>().Publish(new UpdateXmlEventModel());
            StaticVariable.TreeViewState = StaticVariable.TreeViewStateType.Unlock_;
        }
    }
}
