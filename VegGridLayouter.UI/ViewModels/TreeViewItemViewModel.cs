using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using VegGridLayouter.UI.Events;
using VegGridLayouter.UI.ViewModels;

namespace VegGridLayouter.UI.ViewModels
{
    public class AttributeItem
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public AttributeItem(string type, string value)
        {
            this.Type = type;
            this.Value = value;
        }
    }

    public class TreeViewItemViewModel : BindableBase
    {
        public string Header { get; set; }
        public string Value { get; set; }   // 标签的值
        public string Type { get; set; }    // 标签的类型

        public List<AttributeItem> Attributes { get; set; } = new List<AttributeItem>();

        public string Name { get; set; }

        private ObservableCollection<TreeViewItemViewModel> _children = new ObservableCollection<TreeViewItemViewModel>();
        public ObservableCollection<TreeViewItemViewModel> Children
        {
            get { return _children; }
            set { SetProperty(ref _children, value); }
        }


        private bool _isGridRoot = false;

        public bool IsGridRoot
        {
            get { return _isGridRoot; }
            set {
                _isGridRoot = value;
                RaisePropertyChanged(nameof(IsGridRoot));
            }
        }

        private bool _isGrid = false;

        public bool IsGrid
        {
            get { return _isGrid; }
            set
            {
                _isGrid = value;
                RaisePropertyChanged(nameof(IsGrid));
            }
        }

        private bool _isAddChildPopupOpen = false;

        public bool IsAddChildPopupOpen
        {
            get { return _isAddChildPopupOpen; }
            set
            {
                _isAddChildPopupOpen = value;
                RaisePropertyChanged(nameof(IsAddChildPopupOpen));
            }
        }

        private bool _isSetRowColumnPopupOpen = false;

        public bool IsSetRowColumnPopupOpen
        {
            get { return _isSetRowColumnPopupOpen; }
            set
            {
                _isSetRowColumnPopupOpen = value;
                RaisePropertyChanged(nameof(IsSetRowColumnPopupOpen));
            }
        }


        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly IEventAggregator _aggregator;

        public TreeViewItemViewModel(MainWindowViewModel mainWindowViewModel, IEventAggregator eventAggregator)
        {
            this._mainWindowViewModel = mainWindowViewModel;
            AddChildCommand = new DelegateCommand<TreeViewItemViewModel>(AddChild);
            RemoveChildCommand = new DelegateCommand<TreeViewItemViewModel>(RemoveChild);
            ConfirmChildRowColumnCommand = new DelegateCommand(ConfirmChildRowColumn);
            SetRowColumnCommand = new DelegateCommand<TreeViewItemViewModel>(SetRowColumn);

            this._aggregator = eventAggregator;

            _aggregator.GetEvent<AddCheckedGridChildEvent>().Subscribe(addCheckedGridChildrenProcess);
        }

        public DelegateCommand<TreeViewItemViewModel> AddChildCommand { get; private set; }
        public DelegateCommand<TreeViewItemViewModel> RemoveChildCommand { get; private set; }
        public DelegateCommand<TreeViewItemViewModel> SetRowColumnCommand { get; private set; }
        public DelegateCommand ConfirmChildRowColumnCommand { get; set; }

        private int _childRow;

        public int ChildRow
        {
            get { return _childRow; }
            set
            {
                _childRow = value;
                RaisePropertyChanged(nameof(ChildRow));
            }
        }

        private int _childColumn;

        public int ChildColumn
        {
            get { return _childColumn; }
            set
            {
                _childColumn = value;
                RaisePropertyChanged(nameof(ChildColumn));
            }
        }

        private TreeViewItemViewModel _parent;

        public TreeViewItemViewModel Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                RaisePropertyChanged(nameof(Parent));
            }
        }


        private void AddChild(TreeViewItemViewModel item)
        {
            if (IsGridRoot)
            {
                //var newItem = new TreeViewItemViewModel(_mainWindowViewModel) { Header = "网格", Name = "VegGrid" };
                //Children.Add(newItem);
                //_mainWindowViewModel.Code = _mainWindowViewModel.UpdateXml().ToString();
                _mainWindowViewModel.SelectedTreeItem = item;
                item.ChildRow = 0;
                item.ChildColumn = 0;
                item.IsAddChildPopupOpen = !item.IsAddChildPopupOpen;
            }

        }

        public void RemoveChild(TreeViewItemViewModel child)
        {
            _removeChild(_mainWindowViewModel.TreeItems, child);
            _mainWindowViewModel.Code = _mainWindowViewModel.UpdateXml().ToString();
        }

        // TODO: 我觉得需要加一个Parent，现在先不做乐
        private void _removeChild(ObservableCollection<TreeViewItemViewModel> treeItems, TreeViewItemViewModel child)
        {
            if (treeItems.Contains(child)) {
                treeItems.Remove(child);
                return;
            }
            else
            {
                foreach (var item in treeItems)
                {
                    _removeChild(item.Children, child);
                }
            }
            // _mainWindowViewModel.UpdateXml();
        }

        private void ConfirmChildRowColumn()
        {
            var newItem = new TreeViewItemViewModel(_mainWindowViewModel, _aggregator)
            {
                Header = "网格",
                Name = "VegGrid",
                Attributes = new List<AttributeItem>() { new AttributeItem("Row", ChildRow.ToString()), new AttributeItem("Column", ChildColumn.ToString()) },
                Parent = this
            };
            Children.Add(newItem);
            _mainWindowViewModel.Code = _mainWindowViewModel.UpdateXml().ToString();
        }

        

        private void SetRowColumn(TreeViewItemViewModel item)
        {
            _mainWindowViewModel.SelectedTreeItem = item;

            int row = 0, column = 0;
            ObservableCollection<TreeViewItemViewModel> temp = null;
            if (this.Parent == null)
            {
                temp = _mainWindowViewModel.TreeItems;
            }
            else
            {
                temp = this.Parent.Children;
            }

            foreach (var treeItem in temp)
            {
                if (treeItem.Name == "RowDefinitions")
                {
                    row = treeItem.Children.Count;
                }
                else if (treeItem.Name == "ColumnDefinitions")
                {
                    column = treeItem.Children.Count;
                }
            }

            List<CellViewModel> cells = new List<CellViewModel>();

            foreach (var child in this.Children)
            {
                int itemRow = 0, itemColumn = 0;
                foreach (var attributes in child.Attributes)
                {
                    if (attributes.Type == "Row")
                    {
                        itemRow = Int32.Parse(attributes.Value);
                    }
                    else if (attributes.Type == "Column")
                    {
                        itemColumn = Int32.Parse(attributes.Value);
                    }
                }
                cells.Add(new CellViewModel() { Row = itemRow, Column = itemColumn, IsChecked = true });
            }

            RowColumnMessageModel message = new RowColumnMessageModel(this)
            {
                Row = row,
                Column = column,
                ExistedCells = cells,
                MainWindowViewModel = _mainWindowViewModel,
                Code = 200
            };
            _aggregator.GetEvent<RowColumnMessageEvent>().Publish(message);


            item.IsSetRowColumnPopupOpen = !item.IsSetRowColumnPopupOpen;
        }

        private void addCheckedGridChildrenProcess(AddCheckedGridChildMessageModel message)
        {
            foreach (var item in _mainWindowViewModel.TreeItems)
            {
                searchChildAndAdd(item, message.CheckedMessage.Target, message);
            }
        }

        private void searchChildAndAdd(TreeViewItemViewModel search, TreeViewItemViewModel target, AddCheckedGridChildMessageModel message)
        {
            if (search == target)
            {
                foreach (var item in message.CheckedCells)
                {
                    bool flag = false;
                    foreach (var existedItem in message.CheckedMessage.ExistedCells)
                    {
                        if (existedItem.Row == item.Row && existedItem.Column == item.Column) flag = true;
                    }
                    if (flag) continue;

                    var newItem = new TreeViewItemViewModel(_mainWindowViewModel, _aggregator)
                    {
                        Header = "网格",
                        Name = "VegGrid",
                        Attributes = new List<AttributeItem>() { new AttributeItem("Row", item.Row.ToString()), new AttributeItem("Column", item.Column.ToString()) },
                        Parent = target.Parent
                    };

                    target.Children.Add(newItem);
                }
                return;
            }
            else
            {
                foreach (var item in search.Children)
                {
                    item.searchChildAndAdd(item, target, message);
                }
            }
        }

        internal void SetAttributes(XElement element)
        {
            Attributes.Clear();
            foreach (var attribute in element.Attributes())
            {
                Attributes.Add(new AttributeItem(attribute.Name.LocalName, attribute.Value));
            }
        }

        public void UpdateXml()
        {
            _mainWindowViewModel.Code = _mainWindowViewModel.UpdateXml().ToString();
        }
    }

    
}
