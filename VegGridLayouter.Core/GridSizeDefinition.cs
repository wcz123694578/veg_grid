using System.Xml.Serialization;
using VegGridLayouter.Core.Attributes;

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

    [PropertiesControlDescription("行")]
    public class RowDefinition : GridSizeDefinition { }
    [PropertiesControlDescription("列")]
    public class ColumnDefinition : GridSizeDefinition { }
}
