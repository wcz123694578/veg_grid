using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public VegThickness Margin { get; set; } = new VegThickness();        // 内边距

        // 目前Generate放在此处十分不合理
        public virtual void Generate()
        {

        }
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
