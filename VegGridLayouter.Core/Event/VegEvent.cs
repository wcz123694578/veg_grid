using ScriptPortal.Vegas;

namespace VegGridLayouter.Core.Element
{
    public class VegEvent : VegElement
    {
        public VegEvent()
        {
        }

        public VegPosition Position { get; set; } = new VegPosition { X = 0, Y = 0 };
    }
}
