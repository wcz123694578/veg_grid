using ScriptPortal.Vegas;
using VegGridLayouter.Core.Visual;

namespace VegGridLayouter.Core.Element
{
    public class VegEllipse : VegShape
    {
        public VegEllipse()
        {

        }

        public VegEllipse(double radius)
        {
            double height = CurProject.Video.Height;
            double width = CurProject.Video.Width;
            if (width > height)
            {
                MinorAxis = radius;
                MajorAxis = radius * (height / width);
            }
            else
            {
                MinorAxis = radius;
                MajorAxis = radius * (width / height);
            }
        }

        public VegEllipse(double radius, VegColor color) : this(radius)
        {
            this.Color = color;
        }

        public VegEllipse(double radius, VegColor color, VegPosition position) : this(radius, color)
        {
            this.Position = position;
        }

        public VegEllipse(double majorAxis, double minorAxis)
        {
            this.MajorAxis = majorAxis;
            this.MinorAxis = minorAxis;
        }

        public VegEllipse(double majorAxis, double minorAxis, VegColor color) : this(majorAxis, minorAxis)
        {
            this.Color = color;
        }

        public VegEllipse(double majorAxis, double minorAxis, VegColor color, VegPosition position) : this(majorAxis, minorAxis, color)
        {
            this.Position = position;
        }

        public double MajorAxis { get; set; } = 1;
        public double MinorAxis { get; set; } = 1;
        public override Effect maskEffect { get; set; }

        public override void Generate()
        {
            PlugInNode plugIn = CurVegas.Generators.GetChildByUniqueID("{Svfx:com.vegascreativesoftware:solidcolor}");
            VideoEvent videoEvent = VegEventHelper.AddEventWithPlugIn(Track, plugIn);
            ApplyColor(videoEvent);
            PlugInNode maskPlugIn = CurVegas.VideoFX.GetChildByUniqueID("{Svfx:com.vegascreativesoftware:bzmasking}");
            this.maskEffect = VegEventHelper.AddVideoFX(videoEvent, maskPlugIn);

            ApplySize(videoEvent);
        }

        public override void ApplySize(VideoEvent videoEvent)
        {
            OFXChoiceParameter choiceParameter = (OFXChoiceParameter)maskEffect.OFXEffect.Parameters[6];
            choiceParameter.Value = choiceParameter.Choices[0];

            OFXDoubleParameter widthParameter = (OFXDoubleParameter)maskEffect.OFXEffect.Parameters[9];
            OFXDoubleParameter heightParameter = (OFXDoubleParameter)maskEffect.OFXEffect.Parameters[10];

            widthParameter.Value = this.MajorAxis;
            heightParameter.Value = this.MinorAxis;
        }
    }
}
