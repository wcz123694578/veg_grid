using ScriptPortal.Vegas;
using VegGridLayouter.Core.Visual;

namespace VegGridLayouter.Core.Element
{
    public abstract class VegShape : VegEvent
    {
        public VegColor Color { get; set; } = new VegColor { R = 0xff, G = 0xff, B = 0xff, A = 1 };
        public override Effect maskEffect { get; set; }

        protected void ApplyColor(VideoEvent videoEvent)
        {
            OFXRGBAParameter colorParameter = (OFXRGBAParameter)videoEvent.Takes[0].Media.Generator.OFXEffect.Parameters[0];
            colorParameter.Value = new OFXColor
            {
                R = this.Color.R * 1.0 / 255,
                G = this.Color.G * 1.0 / 255,
                B = this.Color.B * 1.0 / 255,
                A = this.Color.A
            };
        }

        public abstract void ApplySize(VideoEvent videoEvent);
    }
}
