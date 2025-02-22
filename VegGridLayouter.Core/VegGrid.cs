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
        // public List<GridChild> Children = new List<GridChild>();
        public VegGridCollection Children { get; set; }
        private bool _isLeaf;



        public VegGrid()
        {
            Children = new VegGridCollection(this);
            Level = 0;
        }



        public void AddRow(RowDefinition row) => RowDefinitions.Add(row);
        public void AddColumn(ColumnDefinition column) => ColumnDefinitions.Add(column);
        public void AddChild(VegGrid child)
        {
            // Children.Add(child);
            child.Parent = this;
            UpdateChildLevels(child, this.Level + 1);
        }

        /// 递归更新 `Level`
        private void UpdateChildLevels(VegGrid node, int level)
        {
            node.Level = level;
            foreach (var subChild in node.Children)
            {
                UpdateChildLevels(subChild, level + 1);
            }
        }

        public void CalculateLayout(double availableWidth, double availableHeight)
        {
            if (this.Level == 0)
            {
                this.ComputedWidth = CurProject.Video.Width;
                this.ComputedHeight = CurProject.Video.Height;
                this.ComputedX = 0;
                this.ComputedY = 0;
                //this.OffsetX = this.OffsetY = 0;
            }

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

                //child.OffsetX = this.OffsetX + child.ComputedX;
                //child.OffsetY = this.OffsetY + child.ComputedY;
            }
        }

        private double[] DistributeSizes(IEnumerable<GridSizeDefinition> definitions, double availableSize)
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

        private List<VideoTrack> _tempChildList = new List<VideoTrack>();

        public override void Generate()
        {
            double width = CurProject.Video.Width;
            double height = CurProject.Video.Height;

            double trackWidth = 1920, trackHeight = 1080;

            if (this.Level == 0)
            {
                CalculateLayout(width, height);
                TempWidth = width;
                TempHeight = height;
            }
            else
            {
                if (this.ComputedWidth > this.ComputedHeight * (width / height))
                {
                    TempWidth = width;
                    TempHeight = width * (this.ComputedHeight / this.ComputedWidth);
                }
                else
                {
                    TempWidth = height * (this.ComputedWidth / this.ComputedHeight);
                    TempHeight = height;
                }
                // CalculateLayout(this.ComputedWidth, this.ComputedHeight);
                CalculateLayout(TempWidth, TempHeight);
            }

            if (this.ComputedWidth * (height / width) > this.ComputedHeight)
            {
                trackWidth = this.ComputedWidth;
                trackHeight = this.ComputedWidth * (height / width);
            }
            else
            {
                trackWidth = this.ComputedHeight * (width / height);
                trackHeight = this.ComputedHeight;
            }

            VideoTrack videoTrack;

            videoTrack = VegTrackHelper.AppendTrack(CurVegas, $"{this.Row} - {this.Column}");
            videoTrack.CompositeNestingLevel = this.Level;

            if (!(Children.Count == 0))
            {
                VegTrack track = new VegTrack(videoTrack);

                VideoTrack tempChild = VegTrackHelper.AppendTrack(CurVegas, "tempChild");
                tempChild.CompositeNestingLevel = this.Level + 1;
                _tempChildList.Add(tempChild);

                track.SetParentSize(trackHeight, trackHeight);
                //track.ParentPosition = new VegPosition(
                //    -width / 2 + this.ComputedX + this.ComputedWidth / 2,
                //    height / 2 - this.ComputedY - this.ComputedHeight / 2
                //);

                track.ParentPosition = new VegPosition(
                    (this.Level == 0) ? 0 : (-Parent.TempWidth / 2 + this.ComputedWidth / 2 + this.ComputedX),
                    (this.Level == 0) ? 0 : (Parent.TempHeight / 2 - this.ComputedHeight / 2 - this.ComputedY));

                PlugInNode maskPlugIn = CurVegas.VideoFX.GetChildByUniqueID("{Svfx:com.vegascreativesoftware:bzmasking}");
                this.maskEffect = VegTrackHelper.AddVideoFX(track, maskPlugIn);

                OFXDoubleParameter widthParameter = (OFXDoubleParameter)maskEffect.OFXEffect.Parameters[9];
                OFXDoubleParameter heightParameter = (OFXDoubleParameter)maskEffect.OFXEffect.Parameters[10];
                widthParameter.Value = this.ComputedWidth / trackWidth;
                heightParameter.Value = this.ComputedHeight / trackHeight;

                foreach (var item in Children)
                {
                    item.Generate();
                }
            }

            //foreach (var item in Children)
            //{

            else
            {

                

                VegTrack track = new VegTrack(
                    videoTrack,
                    trackWidth, trackHeight
                );
                //track.Position = new VegPosition(
                //    -trackWidth / 2 + this.ComputedX + this.ComputedWidth / 2,
                //    trackHeight / 2 - this.ComputedY - this.ComputedHeight / 2);

                track.Position = new VegPosition(
                    (this.Level == 0) ? 0 : (-Parent.TempWidth / 2 + this.ComputedWidth / 2 + this.ComputedX),
                    (this.Level == 0) ? 0 : (Parent.TempHeight / 2 - this.ComputedHeight / 2 - this.ComputedY));

                PlugInNode maskPlugIn = CurVegas.VideoFX.GetChildByUniqueID("{Svfx:com.vegascreativesoftware:bzmasking}");
                this.maskEffect = VegTrackHelper.AddVideoFX(track, maskPlugIn);

                OFXDoubleParameter widthParameter = (OFXDoubleParameter)maskEffect.OFXEffect.Parameters[9];
                OFXDoubleParameter heightParameter = (OFXDoubleParameter)maskEffect.OFXEffect.Parameters[10];
                widthParameter.Value = this.ComputedWidth / trackWidth;
                heightParameter.Value = this.ComputedHeight / trackHeight;

                Random ran = new Random();
                VegBorder border = new VegBorder(width, height, new Visual.VegColor(ran.Next(256), ran.Next(256), ran.Next(256), 1));
                border.Track = track;
                // border.Margin = new VegThickness(10, 10, 10, 10);
                border.Generate();
            //}

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

    public class VegGridCollection : List<VegGrid>
    {
        private VegGrid _parent;

        public VegGridCollection(VegGrid parent)
        {
            _parent = parent;
        }

        public new void Add(VegGrid child)
        {
            base.Add(child);
            _parent.AddChild(child);
        }
    }
}
