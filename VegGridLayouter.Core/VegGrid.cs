using System;
using System.Collections.Generic;
using System.Linq;
using VegGridLayouter.Core.Element;

namespace VegGridLayouter.Core
{
    public class VegGrid : VegElement
    {
        public List<RowDefinition> RowDefinitions { get; } = new List<RowDefinition>();
        public List<ColumnDefinition> ColumnDefinitions { get; } = new List<ColumnDefinition>();
        public List<GridChild> Children = new List<GridChild>();

        public void AddRow(RowDefinition row) => RowDefinitions.Add(row);
        public void AddColumn(ColumnDefinition column) => ColumnDefinitions.Add(column);
        public void AddChild(GridChild child) => Children.Add(child);

        public void CalculateLayout(int availableWidth, int availableHeight)
        {
            /// 计算行列的实际尺寸
            int[] rowHeights = DistributeSizes(RowDefinitions, availableHeight);
            int[] columnWidths = DistributeSizes(ColumnDefinitions, availableWidth);

            // 计算子元素的布局
            foreach (var child in Children)
            {
                int x = columnWidths.Take(child.Column).Sum();
                int y = rowHeights.Take(child.Row).Sum();
                int width = columnWidths.Skip(child.Column).Take(child.ColumnSpan).Sum();
                int height = rowHeights.Skip(child.Row).Take(child.RowSpan).Sum();

                child.ComputedX = x;
                child.ComputedY = y;
                child.ComputedWidth = width;
                child.ComputedHeight = height;
            }
        }

        private int[] DistributeSizes(IEnumerable<GridSizeDefinition> definitions, int availableSize)
        {
            var defList = definitions.ToList();

            int totalFixed = definitions.Where(d => d.Type == GridSizeType.Fixed).Sum(d => d.Value);
            int autoCount = definitions.Count(d => d.Type == GridSizeType.Auto);
            int starSum = definitions.Where(d => d.Type == GridSizeType.Star).Sum(d => d.Value);
            int remaining = availableSize - totalFixed;

            int[] sizes = new int[defList.Count];

            for (int i = 0; i < defList.Count; i++)
            {
                switch (defList[i].Type)
                {
                    case GridSizeType.Fixed:
                        sizes[i] = defList[i].Value;
                        break;
                    case GridSizeType.Auto:
                        // sizes[i] = remaining / Math.Max(1, autoCount); // 简单均分
                        // TODO: 还不能正常计算
                        throw new ArgumentException("还没实现Auto类尺寸");
                        break;
                    case GridSizeType.Star:
                        sizes[i] = (int)(remaining * (defList[i].Value / (float)starSum));
                        break;
                }
            }

            return sizes;
        }

        public override void Generate()
        {
            int width = CurProject.Video.Width;
            int height = CurProject.Video.Height;

            foreach (var item in Children)
            {
                VegTrack track = new VegTrack(
                    CurVegas.Project.AddVideoTrack(),
                    item.ComputedWidth, item.ComputedHeight
                );
                track.Position = new VegPosition(
                    -width / 2 + item.ComputedX + item.ComputedWidth / 2,
                    height / 2 - item.ComputedY - item.ComputedHeight / 2);

                VegBorder border = new VegBorder(width, height);
                border.Track = track;
                border.Generate();
            }
        }
    }

    public class RowDefinition : GridSizeDefinition { }
    public class ColumnDefinition : GridSizeDefinition { }

    public abstract class GridSizeDefinition
    {
        public GridSizeType Type { get; set; }
        public int Value { get; set; }
    }

    public enum GridSizeType
    {
        Fixed,
        Auto,
        Star
    }

    // 子元素
    public class GridChild
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int RowSpan { get; set; } = 1;
        public int ColumnSpan { get; set; } = 1;

        public int ComputedX { get; set; }
        public int ComputedY { get; set; }
        public int ComputedWidth { get; set; }
        public int ComputedHeight { get; set; }
    }
}
