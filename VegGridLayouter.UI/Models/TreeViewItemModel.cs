using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using VegGridLayouter.UI.ViewModels;

namespace VegGridLayouter.UI.Models
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

    public class TreeViewItemModel : BindableBase
    {
        public string Header { get; set; }
        public string Value { get; set; }   // 标签的值
        public string Type { get; set; }    // 标签的类型

        public List<AttributeItem> Attributes { get; set; } = new List<AttributeItem>();

        public string Name { get; set; }

        private ObservableCollection<TreeViewItemModel> _children = new ObservableCollection<TreeViewItemModel>();
        public ObservableCollection<TreeViewItemModel> Children
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

        private readonly MainWindowViewModel _mainWindowViewModel;

        public TreeViewItemModel(MainWindowViewModel mainWindowViewModel)
        {
            this._mainWindowViewModel = mainWindowViewModel;
            AddChildCommand = new DelegateCommand(AddChild);
            RemoveChildCommand = new DelegateCommand<TreeViewItemModel>(RemoveChild);
        }

        public DelegateCommand AddChildCommand { get; private set; }
        public DelegateCommand<TreeViewItemModel> RemoveChildCommand { get; private set; }

        private void AddChild()
        {
            if (IsGridRoot)
            {
                var newItem = new TreeViewItemModel(_mainWindowViewModel) { Header = "网格", Name = "VegGrid" };
                Children.Add(newItem);
                _mainWindowViewModel.Code = _mainWindowViewModel.UpdateXml().ToString();
            }

        }

        private void RemoveChild(TreeViewItemModel child)
        {
            SearchChild(_mainWindowViewModel.TreeItems, child);
            _mainWindowViewModel.Code = _mainWindowViewModel.UpdateXml().ToString();
        }

        // TODO: 我觉得需要加一个Parent，现在先不做乐
        private void SearchChild(ObservableCollection<TreeViewItemModel> treeItems, TreeViewItemModel child)
        {
            if (treeItems.Contains(child)) {
                treeItems.Remove(child);
                return;
            }
            else
            {
                foreach (var item in treeItems)
                {
                    SearchChild(item.Children, child);
                }
            }
            // _mainWindowViewModel.UpdateXml();
        }

        internal void SetAttributes(XElement element)
        {
            Attributes.Clear();
            foreach (var attribute in element.Attributes())
            {
                Attributes.Add(new AttributeItem(attribute.Name.LocalName, attribute.Value));
            }
        }
    }

    
}
