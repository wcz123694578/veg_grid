using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VegGridLayouter.UI.ViewModels;

namespace VegGridLayouter.UI.Events
{
    public class SelectTreeViewItemEvent : PubSubEvent<SelectTreeViewItemEventModel>
    {

    }

    public class SelectTreeViewItemEventModel
    {
        public SelectTreeViewItemEventModel(TreeViewItemViewModel _item)
        {
            this.SelectedItem = _item;
        }
        public TreeViewItemViewModel SelectedItem { get; set; }
    }
}
