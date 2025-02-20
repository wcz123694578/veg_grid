using ScriptPortal.Vegas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
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
            double[] rowHeights = DistributeSizes(RowDefinitions, availableHeight);
            double[] columnWidths = DistributeSizes(ColumnDefinitions, availableWidth);

            // 计算子元素的布局
            foreach (var child in Children)
            {
                double x = columnWidths.Take(child.Column).Sum();
                double y = rowHeights.Take(child.Row).Sum();
                double width = columnWidths.Skip(child.Column).Take(child.ColumnSpan).Sum();
                double height = rowHeights.Skip(child.Row).Take(child.RowSpan).Sum();

                child.ComputedX = x;
                child.ComputedY = y;
                child.ComputedWidth = width;
                child.ComputedHeight = height;
            }
        }

        private double[] DistributeSizes(IEnumerable<GridSizeDefinition> definitions, int availableSize)
        {
            var defList = definitions.ToList();

            int totalFixed = definitions.Where(d => d.Type == GridSizeType.Fixed).Sum(d => d.Value);
            int autoCount = definitions.Count(d => d.Type == GridSizeType.Auto);
            int starSum = definitions.Where(d => d.Type == GridSizeType.Star).Sum(d => d.Value);
            double remaining = availableSize - totalFixed;

            double[] sizes = new double[defList.Count];

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
                        sizes[i] = (remaining * (defList[i].Value / (float)starSum));
                        break;
                }
            }

            return sizes;
        }

        public override void Generate()
        {
            double width = CurProject.Video.Width;
            double height = CurProject.Video.Height;

            foreach (var item in Children)
            {
                //double trackWidth = (item.ComputedWidth > item.ComputedHeight) ? (item.ComputedWidth) : (item.ComputedHeight * (width / height));
                //double trackHeight = (item.ComputedWidth > item.ComputedHeight) ? (item.ComputedWidth * (height / width)) : (item.ComputedHeight);

                double trackWidth = 0, trackHeight = 0;
                if (/*item.ComputedWidth > item.ComputedHeight || */item.ComputedWidth * (height / width) > item.ComputedHeight)
                {
                    trackWidth = item.ComputedWidth;
                    trackHeight = item.ComputedWidth * (height / width);
                }
                else
                {
                    trackWidth = item.ComputedHeight * (width / height);
                    trackHeight = item.ComputedHeight;
                }

                VegTrack track = new VegTrack(
                    CurVegas.Project.AddVideoTrack(),
                    trackWidth, trackHeight
                );
                track.Position = new VegPosition(
                    -width / 2 + item.ComputedX + item.ComputedWidth / 2,
                    height / 2 - item.ComputedY - item.ComputedHeight / 2);

                PlugInNode maskPlugIn = CurVegas.VideoFX.GetChildByUniqueID("{Svfx:com.vegascreativesoftware:bzmasking}");
                this.maskEffect = VegTrackHelper.AddVideoFX(track, maskPlugIn);

                OFXDoubleParameter widthParameter = (OFXDoubleParameter)maskEffect.OFXEffect.Parameters[9];
                OFXDoubleParameter heightParameter = (OFXDoubleParameter)maskEffect.OFXEffect.Parameters[10];
                widthParameter.Value = item.ComputedWidth / trackWidth;
                heightParameter.Value = item.ComputedHeight / trackHeight;

                VegBorder border = new VegBorder(width, height);
                border.Track = track;
                // border.Margin = new VegThickness(10, 10, 10, 10);
                border.Generate();
            }
        }
    }

    public class RowDefinition : GridSizeDefinition { }
    public class ColumnDefinition : GridSizeDefinition { }

    public abstract class GridSizeDefinition
    {
        [XmlAttribute]
        public GridSizeType Type { get; set; }
        [XmlAttribute]
        public int Value { get; set; }
    }

    public enum GridSizeType
    {
        Fixed,
        Auto,
        Star
    }

    // 子元素
    public class GridChild : VegElement
    {
        [XmlAttribute]
        public int Row { get; set; }
        [XmlAttribute]
        public int Column { get; set; }
        [XmlAttribute]
        public int RowSpan { get; set; } = 1;
        [XmlAttribute]
        public int ColumnSpan { get; set; } = 1;

        public double ComputedX { get; set; }
        public double ComputedY { get; set; }
        public double ComputedWidth { get; set; }
        public double ComputedHeight { get; set; }
    }
}
