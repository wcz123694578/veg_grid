using System.Xml.Serialization;

namespace VegGridLayouter.Core
{
    public abstract class GridSizeDefinition
    {
        public GridSizeDefinition()
        {
            this.Type = GridSizeType.Star;
            this.Value = 1;
        }

        [XmlAttribute]
        public GridSizeType Type { get; set; }
        [XmlAttribute]
        public int Value { get; set; }
    }

    public class RowDefinition : GridSizeDefinition { }
    public class ColumnDefinition : GridSizeDefinition { }
}
