using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VegGridLayouter.UI.ViewModels;

namespace VegGridLayouter.UI.Events
{
    public class AddCheckedGridChildEvent : PubSubEvent<AddCheckedGridChildMessageModel>
    {

    }

    public class AddCheckedGridChildMessageModel
    {
        public AddCheckedGridChildMessageModel(RowColumnMessageModel checkedMessage)
        {
            this.CheckedMessage = checkedMessage;
        }

        public RowColumnMessageModel CheckedMessage { get; set; }
        public List<CellViewModel> CheckedCells { get; set; } = new List<CellViewModel>();
    }
}
