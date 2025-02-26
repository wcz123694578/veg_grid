using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VegGridLayouter.UI.ViewModels;

namespace VegGridLayouter.UI.Events
{
    public class RowColumnMessageEvent : PubSubEvent<RowColumnMessageModel>
    {
        
    }

    public class RowColumnMessageModel
    {
        public RowColumnMessageModel(TreeViewItemViewModel target)
        {
            this.Target = target;
        }

        public TreeViewItemViewModel Target { get; set; }
        public MainWindowViewModel MainWindowViewModel { get; set; }

        public int Row { get; set; }
        public int Column { get; set; }
        public int Code { get; set; }

        public List<CellViewModel> ExistedCells { get; set; } = new List<CellViewModel>();
    }
}
