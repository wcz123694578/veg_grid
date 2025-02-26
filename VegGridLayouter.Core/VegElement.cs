using System;
using System.Collections.Generic;
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

        public VegThickness Margin { get; set; } = new VegThickness();        // 内边距

        // 目前Generate放在此处十分不合理
        public virtual void Generate()
        {

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

        internal double TempWidth { get; set; }
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

        public VegThickness(int _left, int _top, int _right, int _bottom)
        {
            this.Left = _left;
            this.Top = _top;
            this.Right = _right;
            this.Bottom = _bottom;
        }

        public int Left { get; set; } = 0;
        public int Top { get; set; } = 0;
        public int Right { get; set; } = 0;
        public int Bottom { get; set; } = 0;
    }
}
