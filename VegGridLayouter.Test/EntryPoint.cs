using ScriptPortal.Vegas;
using System.Windows.Forms;
using VegGridLayouter.Core;
using VegGridLayouter.Parser;

namespace VegGridLayouter.Test
{
    public class EntryPoint
    {
        public void FromVegas(Vegas vegas)
        {
            VegasManager.Instance = vegas;

            try
            {
                TestGrid();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            

            
            // track.ParentPosition = new VegPosition(20, 10);
        }

        public void TestTrack(Vegas vegas)
        {
            VegTrack track = new VegTrack(vegas.Project.AddVideoTrack());
            //track.VegasTrack
            track.Position = new VegPosition(20, 10);

            VideoTrack childTrack = new VideoTrack(vegas.Project.Tracks.Count);
            VegTrack child = new VegTrack(childTrack);
            vegas.Project.Tracks.Add(childTrack);

            childTrack.CompositeNestingLevel = 1;
        }

        public void TestGrid()
        {
            VegGrid grid = new VegGrid();

            grid.AddRow(new RowDefinition() { Type = GridSizeType.Star, Value = 1 });
            grid.AddRow(new RowDefinition() { Type = GridSizeType.Star, Value = 2 });

            grid.AddColumn(new ColumnDefinition() { Type = GridSizeType.Star, Value = 1 });

            grid.Children.Add(new VegGrid() { Row = 0 });

            VegGrid child2 = new VegGrid();
            child2.AddRow(new RowDefinition() { Type = GridSizeType.Star, Value = 1 });
            child2.AddColumn(new ColumnDefinition() { Type = GridSizeType.Star, Value = 1 });
            child2.AddColumn(new ColumnDefinition() { Type = GridSizeType.Star, Value = 1 });
            child2.Children.Add(new VegGrid() { Row = 0 });

            VegGrid child2_1 = new VegGrid();
            child2_1.AddRow(new RowDefinition() { Type = GridSizeType.Star, Value = 2 });
            child2_1.AddRow(new RowDefinition() { Type = GridSizeType.Star, Value = 1 });
            child2_1.AddColumn(new ColumnDefinition() { Type = GridSizeType.Star, Value = 1 });
            child2_1.Children.Add(new VegGrid() { Row = 0 });
            child2_1.Children.Add(new VegGrid() { Row = 1 });

            child2.Children.Add(child2_1);
            child2_1.Column = 1;

            child2.Row = 1;
            grid.Children.Add(child2);


            VegXmlDeserializer.SerializeXml(grid);

            grid.Generate();
        }
    }
}
