using ScriptPortal.Vegas;
using VegGridLayouter.Core;
using VegGridLayouter.Core.Element;
using VegGridLayouter.Core.Visual;

namespace VegGridLayouter.Test
{
    public class EntryPoint
    {
        public void FromVegas(Vegas vegas)
        {
            VegasManager.Instance = vegas;

            VegGrid grid = new VegGrid();
            grid.AddRow(new RowDefinition { Type = GridSizeType.Fixed, Value = 50 });
            grid.AddRow(new RowDefinition { Type = GridSizeType.Star, Value = 1 });
            grid.AddRow(new RowDefinition { Type = GridSizeType.Star, Value = 2 });

            grid.AddColumn(new ColumnDefinition { Type = GridSizeType.Fixed, Value = 100 });
            grid.AddColumn(new ColumnDefinition { Type = GridSizeType.Star, Value = 2 });
            grid.AddColumn(new ColumnDefinition { Type = GridSizeType.Star, Value = 1 });

            grid.AddChild(new GridChild { Row = 0, Column = 0 });
            grid.AddChild(new GridChild { Row = 1, Column = 1, ColumnSpan = 2 });
            grid.AddChild(new GridChild { Row = 2, Column = 2 });

            int width = grid.CurProject.Video.Width;
            int height = grid.CurProject.Video.Height;

            grid.CalculateLayout(width, height);

            grid.Generate();
        }
    }
}
