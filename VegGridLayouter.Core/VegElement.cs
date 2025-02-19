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

        // 目前Generate放在此处十分不合理
        public virtual void Generate()
        {

        }
    }
}
