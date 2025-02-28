using ScriptPortal.Vegas;
using VegGridLayouter.Core.Visual;

namespace VegGridLayouter.Core.Element
{
    public class VegBorder : VegShape
    {
        public VegBorder()
        {

        }

        public VegBorder(double width, double height)
        {
            this.Height = height;
            this.Width = width;
        }

        public VegBorder(double width, double height, VegColor color) : this(width, height)
        {
            this.Color = color;
        }

        public VegBorder(double width, double height, VegColor color, VegPosition position) : this(width, height, color)
        {
            this.Position = position;
        }

        public double Height { get; set; } = 1;
        public double Width { get; set; } = 1;
        public override Effect maskEffect { get; set; }

        public override void Generate()
        {
            //string str = "";
            //foreach (var plugIn in CurVegas.Instance.Generators)
            //{
            //    str += $"{plugIn.UniqueID}, {plugIn.Name}\n";
            //}
            //File.WriteAllText("res.txt", str);
            PlugInNode plugIn = CurVegas.Generators.GetChildByUniqueID("{Svfx:com.vegascreativesoftware:solidcolor}");

            // TODO: 还没想好圆角要怎么加，每个创建的轨道都来一个软对比？还是动态地调整，当CornerRadius>0时才加？
            // TODO: o，还是再来个派生类好了。。。

            VideoEvent videoEvent = VegEventHelper.AddEventWithPlugIn(Track, plugIn);
            // Take take = videoEvent.AddTake(videoEvent.Takes[0].Media.GetVideoStreamByIndex(0));

            ApplyColor(videoEvent);

            PlugInNode maskPlugIn = CurVegas.VideoFX.GetChildByUniqueID("{Svfx:com.vegascreativesoftware:bzmasking}");
            this.maskEffect = VegEventHelper.AddVideoFX(videoEvent, maskPlugIn);
            ApplySize(videoEvent);
        }

        public override string ToString()
        {
            string colorString = Color.ToString();
            string positionString = Position.ToString();
            return $"Height: {this.Height}\nWidth: {this.Width}\n"
                + Color.ToString()
                + Position.ToString();
        }

        public override void ApplySize(VideoEvent videoEvent)
        {
            
            OFXDoubleParameter widthParameter = (OFXDoubleParameter)maskEffect.OFXEffect["Width_0"];
            OFXDoubleParameter heightParameter = (OFXDoubleParameter)maskEffect.OFXEffect["Height_0"];

            widthParameter.Value = (this.Width - (Margin.Left + Margin.Right) * (CurProject.Video.Width / this.Track.Width)) / this.Width;
            heightParameter.Value = (this.Height - (Margin.Top + Margin.Bottom) * (CurProject.Video.Height / this.Track.Height)) / this.Height;
        }
    }
}
