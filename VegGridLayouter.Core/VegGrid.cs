using ScriptPortal.Vegas;
using System;
using System.Collections.Generic;
using System.Linq;
using VegGridLayouter.Core.Element;
using static VegGridLayouter.Core.VegasContextFactory;

namespace VegGridLayouter.Core
{
    public class VegGrid : VegElement
    {
        public List<RowDefinition> RowDefinitions { get; set; } = new List<RowDefinition>();

        public List<ColumnDefinition> ColumnDefinitions { get; set; } = new List<ColumnDefinition>();

        // public List<GridChild> Children = new List<GridChild>();
        public VegGridCollection Children { get; set; }

        public VegGrid()
        {
            Children = new VegGridCollection(this);
            Level = 0;

            if (this.Level == 0)
            {
                this.ComputedWidth = Context.Project.Video.Width;
                this.ComputedHeight = Context.Project.Video.Height;
                this.ComputedX = 0;
                this.ComputedY = 0;
                //this.OffsetX = this.OffsetY = 0;
            }
        }

        public void AddRow(RowDefinition row) => RowDefinitions.Add(row);
        public void AddColumn(ColumnDefinition column) => ColumnDefinitions.Add(column);
        public void SetChild(VegGrid child)
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

        /// <summary>
        /// 计算行列的实际尺寸，并为每个子元素计算布局位置和尺寸
        /// </summary>
        /// <param name="availableWidth"></param>
        /// <param name="availableHeight"></param>
        public void CalculateLayout(double availableWidth, double availableHeight)
        {
            /// 计算行列的实际尺寸
            double[] rowHeights = CalculateAxisSize(RowDefinitions, availableHeight);
            double[] columnWidths = CalculateAxisSize(ColumnDefinitions, availableWidth);

            // 计算子元素的布局
            foreach (var child in Children)
            {
                ApplyChildLayout(child, rowHeights, columnWidths);

                //child.OffsetX = this.OffsetX + child.ComputedX;
                //child.OffsetY = this.OffsetY + child.ComputedY;
            }
        }

        /// <summary>
        /// 将算得的尺寸应用到轨道位置和尺寸中
        /// </summary>
        /// <param name="child"></param>
        /// <param name="rowHeights"></param>
        /// <param name="columnWidths"></param>
        private void ApplyChildLayout(VegGrid child, double[] rowHeights, double[] columnWidths)
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

        /// <summary>
        /// 分配行/列的实际尺寸
        /// </summary>
        /// <param name="definitions">行/列定义</param>
        /// <param name="availableSize">可用空间</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private double[] CalculateAxisSize(IEnumerable<GridSizeDefinition> definitions, double availableSize)
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

        /// <summary>
        /// 获取当前元素在Vegas中的实际位置（以父元素中心为原点）
        /// </summary>
        /// <returns></returns>
        public VegPosition TrackPosition
        {
            get
            {
                double finalX = (this.Level == 0) ? 0 : (-Parent.TempWidth / 2 + this.ComputedWidth / 2 + this.ComputedX);
                double finalY = (this.Level == 0) ? 0 : (Parent.TempHeight / 2 - this.ComputedHeight / 2 - this.ComputedY);
                return new VegPosition(
                    finalX - (Margin.GetLeftRight() / 2 - Margin.Left),
                    finalY + (Margin.GetTopBottom() / 2 - Margin.Top)
                );
            }
        }

        public override void Generate()
        {
            base.Generate();

            double width = Context.Project.Video.Width;
            double height = Context.Project.Video.Height;

            CalculateLayout(TempWidth, TempHeight);

            

            VideoTrack videoTrack;

            videoTrack = VegTrackHelper.AppendTrack($"{this.Row} - {this.Column}");
            videoTrack.CompositeNestingLevel = this.Level;

            VegPosition trackPosition = TrackPosition;

            if (!(Children.Count == 0))
            {
                VegTrack track = new VegTrack(videoTrack);

                VideoTrack tempChild = VegTrackHelper.AppendTrack("tempChild");
                tempChild.CompositeNestingLevel = this.Level + 1;

                track.SetParentSize(TrackHeight, TrackHeight);

                track.ParentPosition = trackPosition;

                PlugInNode maskPlugIn = Context.VideoFX.GetChildByUniqueID("{Svfx:com.vegascreativesoftware:bzmasking}");
                this.maskEffect = VegTrackHelper.AddVideoFX(track, maskPlugIn);

                ModifyMaskEffect(maskEffect);

                foreach (var item in Children)
                {
                    item.Generate();
                }
            }

            else
            {
                VegTrack track = new VegTrack(
                    videoTrack,
                    TrackWidth, TrackHeight
                );

                track.Position = trackPosition;

                PlugInNode maskPlugIn = Context.VideoFX.GetChildByUniqueID("{Svfx:com.vegascreativesoftware:bzmasking}");
                this.maskEffect = VegTrackHelper.AddVideoFX(track, maskPlugIn);

                ModifyMaskEffect(maskEffect);

                Random ran = new Random();
                VegBorder border = new VegBorder(
                    width, height, 
                    new Visual.VegColor(
                        (Level % 3 == 0) ? ran.Next(256) : 0, 
                        (Level % 3 == 1) ? ran.Next(256) : 0, 
                        (Level % 3 == 2) ? ran.Next(256) : 0, 
                        1
                    )
                );
                border.Track = track;
                // border.Margin = new VegThickness(10, 10, 10, 10);
                border.Generate();

            }
        }

        private void ModifyMaskEffect(Effect maskEffect)
        {
            OFXDoubleParameter widthParameter = (OFXDoubleParameter)maskEffect.OFXEffect["Width_0"];
            OFXDoubleParameter heightParameter = (OFXDoubleParameter)maskEffect.OFXEffect["Height_0"];
            widthParameter.Value = (this.ComputedWidth) / TrackWidth;
            heightParameter.Value = (this.ComputedHeight) / TrackHeight;
        }
    }

    

    public enum GridSizeType
    {
        Fixed,
        Auto,
        Star
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
            _parent.SetChild(child);
        }
    }
}
