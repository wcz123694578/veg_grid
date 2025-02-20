using ScriptPortal.Vegas;
using System.IO;
using VegGridLayouter.Core;
using VegGridLayouter.Core.Element;
using VegGridLayouter.Core.Visual;
using VegGridLayouter.Parser;

namespace VegGridLayouter.Test
{
    public class EntryPoint
    {
        public void FromVegas(Vegas vegas)
        {
            VegasManager.Instance = vegas;

            VegGrid grid = new VegGrid();
            //grid.AddRow(new RowDefinition() { Type = GridSizeType.Star, Value = 1 });
            //grid.AddRow(new RowDefinition() { Type = GridSizeType.Star, Value = 1 });
            //grid.AddRow(new RowDefinition() { Type = GridSizeType.Star, Value = 1 });
            //grid.AddRow(new RowDefinition() { Type = GridSizeType.Star, Value = 1 });

            //grid.AddColumn(new ColumnDefinition() { Type = GridSizeType.Star, Value = 1 });
            //grid.AddColumn(new ColumnDefinition() { Type = GridSizeType.Star, Value = 2 });
            //grid.AddColumn(new ColumnDefinition() { Type = GridSizeType.Star, Value = 1 });

            //grid.AddChild(new GridChild());
            //grid.AddChild(new GridChild() { Row = 1 });
            //grid.AddChild(new GridChild() { Row = 2, Column = 1 });
            //grid.AddChild(new GridChild() { Row = 3, Column = 2 });

            FileStream fs = File.Open("test.xml", FileMode.Open);
            grid = VegXmlDeserializer.DeserializeXml(fs);

            int width = grid.CurProject.Video.Width;
            int height = grid.CurProject.Video.Height;

            grid.CalculateLayout(width, height);

            // string xml = VegXmlDeserializer.SerializeXml(grid);

            grid.Generate();
        }
    }
}
