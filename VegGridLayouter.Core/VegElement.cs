using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VegGridLayouter.Core
{
    public class VegElement : VegGridLayouterObject
    {
        public VegElement()
        {
            
        }

        public enum HAlign
        {
            Left = 0,
            Center = 1,
            Right = 2,
            Stretch = 3
        }

        public enum VAlign
        {
            Top = 0,
            Center = 1,
            Bottom = 2,
            Stretch = 3
        }

        public enum Visibility
        {
            Visible = 0,
            Hidden
        }

        private VegElement _parent;

        internal VegElement Parent
        {
            get
            {
                return _parent;
            }

            set
            {
                _parent = value;
            }
        }

        internal int Level { get; set; }

        private string _marginString;

        [XmlAttribute("Margin")]
        public string MarginString {
            get
            {
                return _marginString;
            }
            set
            {
                _marginString = value;
                Margin = _marginString;
            } 
        }

        [XmlIgnore]
        public VegThickness Margin { get; set; } = new VegThickness();        // 内边距

        // 目前Generate放在此处十分不合理
        public virtual void Generate()
        {
            double width = CurProject.Video.Width;
            double height = CurProject.Video.Height;

            this.ComputedWidth -= this.Margin.GetLeftRight();
            this.ComputedHeight -= this.Margin.GetTopBottom();

            // TODO: 
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

            if (this.ComputedWidth * (height / width) > this.ComputedHeight)
            {
                TrackWidth = this.ComputedWidth;
                TrackHeight = this.ComputedWidth * (height / width);
            }
            else
            {
                TrackWidth = this.ComputedHeight * (width / height);
                TrackHeight = this.ComputedHeight;
            }
        }

        [XmlAttribute]
        public int Row { get; set; }
        [XmlAttribute]
        public int Column { get; set; }
        [XmlAttribute]
        public int RowSpan { get; set; } = 1;
        [XmlAttribute]
        public int ColumnSpan { get; set; } = 1;

        internal double ComputedX { get; set; }
        internal double ComputedY { get; set; }
        internal double ComputedWidth { get; set; }
        internal double ComputedHeight { get; set; }

        internal double TempWidth { get; set; }         // TempWidth和TempHeight是递归到某一层后
                                                        // 按父级对这个轨道的计算尺寸的比例换算成能够铺满整个轨道的尺寸
                                                        // 因为子母轨下面的轨道是按照工程尺寸来的
        internal double TempHeight { get; set; }
        internal double TrackWidth { get; set; }
        internal double TrackHeight { get; set; }

        //internal double OffsetX { get; set; }
        //internal double OffsetY { get; set; }
    }

    public class VegThickness
    {
        public VegThickness()
        {

        }

        public VegThickness(double _left, double _top, double _right, double _bottom)
        {
            this.Left = _left;
            this.Top = _top;
            this.Right = _right;
            this.Bottom = _bottom;
        }

        public VegThickness(double _left_right, double _top_bottom)
        {
            this.Left = this.Right = _left_right;
            this.Top = this.Bottom = _top_bottom;
        }

        public VegThickness(double _left_top_right_bottom)
        {
            this.Left = this.Top = this.Bottom = this.Right = _left_top_right_bottom;
        }

        public double Left { get; set; } = 0;
               
        public double Top { get; set; } = 0;
              
        public double Right { get; set; } = 0;
               
        public double Bottom { get; set; } = 0;

        public object ConvertFrom(string value)
        {
            if (value is string str)
            {
                var parts = str.Split(',');

                switch (parts.Length)
                {
                    case 1 when double.TryParse(parts[0], out double all):
                        return new VegThickness(all); // 统一四个方向

                    case 2 when double.TryParse(parts[0], out double leftRight) && double.TryParse(parts[1], out double topBottom):
                        return new VegThickness(leftRight, topBottom); // 左右，上下

                    case 4 when double.TryParse(parts[0], out double left) &&
                               double.TryParse(parts[1], out double top) &&
                               double.TryParse(parts[2], out double right) &&
                               double.TryParse(parts[3], out double bottom):
                        return new VegThickness(left, top, right, bottom); // 四个值分别设置

                    default:
                        throw new ArgumentException("错误的长度数量. 仅支持 'L,T,R,B'、 'LR,TB' 和 'ALL'");
                }
            }
            throw new ArgumentException("字符串解析错误，仅支持string类型");
        }

        public double GetLeftRight() => this.Left + this.Right;
        public double GetTopBottom() => this.Top + this.Bottom;

        public static implicit operator VegThickness(string value)
        {
            VegThickness thickness = new VegThickness();
            return (VegThickness)thickness.ConvertFrom(value);
        }
    }
}
