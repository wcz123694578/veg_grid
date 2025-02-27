using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VegGridLayouter.UI.Events;

namespace VegGridLayouter.UI.ViewModels
{
    public class SetRowColumnPopupViewModel : BindableBase
    {
        private int _totalRowCount;

        public int TotalRowCount
        {
            get { return _totalRowCount; }
            set
            {
                _totalRowCount = value;
                RaisePropertyChanged(nameof(TotalRowCount));
            }
        }

        private int _totalColumnCount;

        public int TotalColumnCount
        {
            get { return _totalColumnCount; }
            set
            {
                _totalColumnCount = value;
                RaisePropertyChanged(nameof(TotalColumnCount));
            }
        }

        private int _wrapPanelWidth;

        public int WrapPanelWidth
        {
            get { return _wrapPanelWidth; }
            set
            {
                _wrapPanelWidth = value;
                RaisePropertyChanged(nameof(WrapPanelWidth));
            }
        }

        public DelegateCommand ConfirmCheckedCellsCommand { get; set; }

        public ObservableCollection<CellViewModel> Cells { get; set; } = new ObservableCollection<CellViewModel>();

        private IEventAggregator _aggregator;

        public SetRowColumnPopupViewModel(IEventAggregator eventAggregator)
        {
            this._aggregator = eventAggregator;
            _aggregator.GetEvent<RowColumnMessageEvent>().Subscribe(EventProcessing);
            // _aggregator.GetEvent<RowColumnMessageEvent>().Subscribe(confirmCheckedCells);

            ConfirmCheckedCellsCommand = new DelegateCommand(confirmCheckedCells);
        }

        private RowColumnMessageModel _internalMessage;

        public void EventProcessing(RowColumnMessageModel message)
        {
            this.TotalRowCount = message.Row;
            this.TotalColumnCount = message.Column;
            this.WrapPanelWidth = 50 * this.TotalRowCount;

            this._internalMessage = message;

            Cells.Clear();
            for (int row = 0; row < TotalRowCount; row++)
            {
                for (int column = 0; column < TotalColumnCount; column++)
                {
                    var cell = new CellViewModel() { Row = row, Column = column };
                    
                    Cells.Add(cell);
                }
            }

            if (message.ExistedCells.Count > 0)
            {
                foreach (var item in message.ExistedCells)
                {
                    Cells[(item.Row) * this.TotalColumnCount + item.Column].IsChecked = true;
                }
            }
        }

        private void confirmCheckedCells()
        {
            string existed = "";
            foreach (var item in _internalMessage.ExistedCells)
            {
                if (Cells[(item.Row) * this.TotalColumnCount + item.Column].IsChecked == false && item.IsChecked == true)
                {
                    existed += $"({item.Row}, {item.Column})\n";
                }
            }

            var res1 = MessageBox.Show("确定要设置所选网格吗？", "消息框", MessageBoxButton.OKCancel);
            AddCheckedGridChildMessageModel addMessage = new AddCheckedGridChildMessageModel(_internalMessage);

            if (res1 == MessageBoxResult.Cancel) return;
            if (res1 == MessageBoxResult.OK)
            {
                string message = $"你取消勾选了已存在的\n{existed}网格，确定要删除吗？";
                var res2 = MessageBox.Show(message, "消息框", MessageBoxButton.OKCancel);

                if (res2 == MessageBoxResult.Cancel)
                {
                    foreach (var item in _internalMessage.ExistedCells)
                    {
                        Cells[(item.Row) * this.TotalColumnCount + item.Column].IsChecked = true;
                    }
                    return;
                }
            }

            var treeItem = _internalMessage.Target;

            foreach (var checkedItem in Cells)
            {
                bool flag = false;
                if (checkedItem.IsChecked)
                { 
                    foreach (var existedItem in _internalMessage.ExistedCells)
                    {
                        if (checkedItem.Row == existedItem.Row && checkedItem.Column == existedItem.Column)
                        {
                            flag = true;
                            break;
                        }
                    }
                }

                else
                {
                    foreach (var existedItem in _internalMessage.ExistedCells)
                    {
                        if (checkedItem.Row == existedItem.Row && checkedItem.Column == existedItem.Column)
                        {
                            if (existedItem.IsChecked)
                            {
                                TreeViewItemViewModel deleteItem = null;
                                foreach (var item in treeItem.Children)
                                {
                                    int tempRow = 0, tempColumn = 0;
                                    foreach (var attribute in item.Attributes)
                                    {
                                        if (attribute.Type == "Row") tempRow = Int32.Parse(attribute.Value);
                                        else if (attribute.Type == "Column") tempColumn = Int32.Parse(attribute.Value);
                                    }
                                    if (tempRow == existedItem.Row && tempColumn == existedItem.Column)
                                    {
                                        deleteItem = item; break;
                                    }
                                }
                                if (deleteItem != null)
                                {
                                    treeItem.Children.Remove(deleteItem);
                                    //_internalMessage.Target.UpdateXml();
                                }
                            }
                            
                        }
                    }
                    flag = true;
                    
                }

                if (flag) continue;

                treeItem.Children.Add(new TreeViewItemViewModel(_internalMessage.MainWindowViewModel, _aggregator)
                {
                    Attributes = new ObservableCollection<AttributeItem>()
                    {
                        new AttributeItem("Row", checkedItem.Row.ToString()),
                        new AttributeItem("Column", checkedItem.Column.ToString())
                    },
                    Header = "网格",
                    Name = "VegGrid",
                    Parent = treeItem
                });
                
            }
            StaticVariable.TreeViewState = StaticVariable.TreeViewStateType.Lock_;
            _aggregator.GetEvent<UpdateXmlEvent>().Publish(new UpdateXmlEventModel());
            StaticVariable.TreeViewState = StaticVariable.TreeViewStateType.Unlock_;
        }
    }

    public class CellViewModel : BindableBase
    {
        public int Row { get; set; }
        public int Column { get; set; }
        private bool _isChecked = false;

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                RaisePropertyChanged(nameof(IsChecked));
            }
        }
    }
}
