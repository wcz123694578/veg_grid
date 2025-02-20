using ScriptPortal.Vegas;

namespace VegGridLayouter.Core
{
    public class VegTrack : VegGridLayouterObject
    {
        public VegTrack(VideoTrack track)
        {
            this.VegasTrack = track;
        }

        public VegTrack(VideoTrack track, double width, double height) : this(track)
        {
            this.Width = width;
            this.Height = height;
        }

        public VegTrack(VideoTrack track, double width, double height, double alpha) : this(track, width, height)
        {
            this.Alpha = alpha;
        }

        private double width;

        public double Width
        {
            get { return width; }
            set {
                width = value;
                this.VegasTrack.TrackMotion.MotionKeyframes[0].Width = value;
            }
        }

        private double height;

        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                this.VegasTrack.TrackMotion.MotionKeyframes[0].Height = value;
            }
        }

        private double alpha;

        public double Alpha
        {
            get { return alpha; }
            set
            {
                alpha = value;
                this.VegasTrack.CompositeLevel = (float)value;
            }
        }

        private VegPosition position;

        public VegPosition Position
        {
            get { return position; }
            set {
                position = value;
                this.VegasTrack.TrackMotion.MotionKeyframes[0].PositionX = value.X;
                this.VegasTrack.TrackMotion.MotionKeyframes[0].PositionY = value.Y;
            }
        }

        public override Effect maskEffect { get; set; }

        public VideoTrack VegasTrack { get; set; }
    }
}
