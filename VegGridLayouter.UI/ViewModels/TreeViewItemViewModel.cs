using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using VegGridLayouter.UI.Events;
using VegGridLayouter.UI.ViewModels;

namespace VegGridLayouter.UI.ViewModels
{
    public class AttributeItem : BindableBase
    {
        public string Type { get; set; }
        private string _value;

        private IEventAggregator _aggregator = StaticVariable.eventAggregator;

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged(nameof(Value));

                // _aggregator.GetEvent<UpdateXmlEvent>().Publish(new UpdateXmlEventModel());

            }
        }

        public AttributeItem(string type, string value)
        {
            this.Type = type;
            this.Value = value;
        }
    }

    public class TreeViewItemViewModel : BindableBase, IEnumerable<TreeViewItemViewModel>
    {
        public string Header { get; set; }

        // 这几个没用了
        public string Value { get; set; }   // 标签的值
        public string Type { get; set; }    // 标签的类型

        private ObservableCollection<AttributeItem> _attributes = new ObservableCollection<AttributeItem>();

        public ObservableCollection<AttributeItem> Attributes
        {
            get { return _attributes; }
            set
            {
                _attributes = value;
                RaisePropertyChanged(nameof(Attributes));
            }
        }


        public string Name { get; set; }

        private ObservableCollection<TreeViewItemViewModel> _children = new ObservableCollection<TreeViewItemViewModel>();
        public ObservableCollection<TreeViewItemViewModel> Children
        {
            get { return _children; }
            set {
                _children = value;
                RaisePropertyChanged(nameof(Children));
            }
        }


        private bool _isCollection = false;

        public bool IsCollection
        {
            get { return _isCollection; }
            set {
                _isCollection = value;
                RaisePropertyChanged(nameof(IsCollection));
            }
        }

        private bool _isChild = false;

        public bool IsChild
        {
            get { return _isChild; }
            set
            {
                _isChild = value;
                RaisePropertyChanged(nameof(IsChild));
            }
        }

        private bool _isGridRoot = false;

        public bool IsGridRoot
        {
            get { return _isGridRoot; }
            set
            {
                _isGridRoot = value;
                RaisePropertyChanged(nameof(IsGridRoot));
            }
        }

        private bool _isNotDefaultSet = true;

        public bool IsNotDefaultSet
        {
            get { return _isNotDefaultSet; }
            set
            {
                _isNotDefaultSet = value;
                RaisePropertyChanged(nameof(IsNotDefaultSet));
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

        private bool _isAddRowPopupOpen = false;

        public bool IsAddRowPopupOpen
        {
            get { return _isAddRowPopupOpen; }
            set
            {
                _isAddRowPopupOpen = value;
                RaisePropertyChanged(nameof(IsAddRowPopupOpen));
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
            SetDefaultCommand = new DelegateCommand<TreeViewItemViewModel>(SetDefault);

            this._aggregator = eventAggregator;

            _aggregator.GetEvent<AddCheckedGridChildEvent>().Subscribe(addCheckedGridChildrenProcess);
        }


        public DelegateCommand<TreeViewItemViewModel> AddChildCommand { get; private set; }
        public DelegateCommand<TreeViewItemViewModel> RemoveChildCommand { get; private set; }
        public DelegateCommand<TreeViewItemViewModel> SetRowColumnCommand { get; private set; }
        public DelegateCommand<TreeViewItemViewModel> SetDefaultCommand { get; set; }
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
            _mainWindowViewModel.SelectedTreeItem = item;

            if (item.Name == "RowDefinitions" || item.Name == "ColumnDefinitions")
            {
                item.IsAddRowPopupOpen = !item.IsAddRowPopupOpen;
                _aggregator.GetEvent<SelectTreeViewItemEvent>().Publish(new SelectTreeViewItemEventModel(item));
            }

            if (this.Name == "Children")
            {
                //var newItem = new TreeViewItemViewModel(_mainWindowViewModel) { Header = "网格", Name = "VegGrid" };
                //Children.Add(newItem);
                //_mainWindowViewModel.Code = _mainWindowViewModel.UpdateXml().ToString();
                
                item.ChildRow = 0;
                item.ChildColumn = 0;
                item.IsAddChildPopupOpen = !item.IsAddChildPopupOpen;
            }

        }

        public void RemoveChild(TreeViewItemViewModel child)
        {
            _removeChild(_mainWindowViewModel.TreeItems, child);

            StaticVariable.TreeViewState = StaticVariable.TreeViewStateType.Lock_;
            _aggregator.GetEvent<UpdateXmlEvent>().Publish(new UpdateXmlEventModel());
            StaticVariable.TreeViewState = StaticVariable.TreeViewStateType.Unlock_;
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
                Attributes = new ObservableCollection<AttributeItem>() { new AttributeItem("Row", ChildRow.ToString()), new AttributeItem("Column", ChildColumn.ToString()) },
                Parent = this
            };
            Children.Add(newItem);
            StaticVariable.TreeViewState = StaticVariable.TreeViewStateType.Lock_;
            _aggregator.GetEvent<UpdateXmlEvent>().Publish(new UpdateXmlEventModel());
            StaticVariable.TreeViewState = StaticVariable.TreeViewStateType.Unlock_;
            _isAddChildPopupOpen = !_isAddChildPopupOpen;
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
                        Attributes = new ObservableCollection<AttributeItem>() { new AttributeItem("Row", item.Row.ToString()), new AttributeItem("Column", item.Column.ToString()) },
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


        private void SetDefault(TreeViewItemViewModel item)
        {
            if (item.Name == "VegGrid")
            {
                MessageBoxResult res = MessageBox.Show("是否需要生成嵌套网格？", "消息框", MessageBoxButton.YesNo);

                if (res == MessageBoxResult.Yes)
                {
                    HashSet<string> existingNames = new HashSet<string>();

                    foreach (var child in item.Children)
                    {
                        existingNames.Add(child.Name);
                    }

                    TreeViewItemViewModel rowDefinitions = new TreeViewItemViewModel(_mainWindowViewModel, _aggregator)
                    {
                        Children = new ObservableCollection<TreeViewItemViewModel>(),
                        Header = "行定义",
                        Name = "RowDefinitions",
                        IsCollection = true,
                        IsNotDefaultSet = false,
                    };
                    TreeViewItemViewModel columnDefinitions = new TreeViewItemViewModel(_mainWindowViewModel, _aggregator)
                    {
                        Children = new ObservableCollection<TreeViewItemViewModel>(),
                        Header = "列定义",
                        Name = "ColumnDefinitions",
                        IsCollection = true,
                        IsNotDefaultSet = false,
                    };
                    TreeViewItemViewModel children_ = new TreeViewItemViewModel(_mainWindowViewModel, _aggregator)
                    {
                        Children = new ObservableCollection<TreeViewItemViewModel>(),
                        Header = "嵌套网格",
                        Name = "Children",
                        IsCollection = true,
                        IsNotDefaultSet = false,
                        IsGridRoot = true
                    };

                    foreach (TreeViewItemViewModel child in new List<TreeViewItemViewModel>() { rowDefinitions, columnDefinitions, children_ })
                    {
                        if (existingNames.Contains(child.Name))
                        {
                            continue;
                        }
                        item.Children.Add(child);
                    }
                }

                HashSet<string> existingAttribute = new HashSet<string>();
                foreach (var attribute in item.Attributes)
                {
                    existingAttribute.Add(attribute.Type);
                }

                AttributeItem row = new AttributeItem("Row", "0");
                AttributeItem column = new AttributeItem("Column", "0");
                AttributeItem rowSpan = new AttributeItem("RowSpan", "1");
                AttributeItem columnSpan = new AttributeItem("ColumnSpan", "1");

                foreach (AttributeItem attribute in new List<AttributeItem>() { row, column, rowSpan, columnSpan })
                {
                    if (existingAttribute.Contains(attribute.Type))
                    {
                        continue;
                    }
                    item.Attributes.Add(attribute);
                }

                if (res == MessageBoxResult.No)
                {
                    item.IsNotDefaultSet = false;
                }
            }

            else if (item.Name == "RowDefinition" || item.Name == "ColumnDefinition")
            {
                HashSet<string> existingAttribute = new HashSet<string>();
                foreach (var attribute in item.Attributes)
                {
                    existingAttribute.Add(attribute.Type);
                }

                AttributeItem type_ = new AttributeItem("Type", "Star");
                AttributeItem value_ = new AttributeItem("Value", "1");

                foreach (AttributeItem attribute in new List<AttributeItem>() { type_, value_ })
                {
                    if (existingAttribute.Contains(attribute.Type))
                    {
                        continue;
                    }
                    item.Attributes.Add(attribute);
                }
                item.IsNotDefaultSet = false;
            }

            StaticVariable.TreeViewState = StaticVariable.TreeViewStateType.Lock_;
            _aggregator.GetEvent<UpdateXmlEvent>().Publish(new UpdateXmlEventModel());
            StaticVariable.TreeViewState = StaticVariable.TreeViewStateType.Unlock_;
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

        public IEnumerator<TreeViewItemViewModel> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    
}
