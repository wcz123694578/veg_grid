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
            SetSize(width, height);
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

        private double parentWidth;

        public double ParentWidth
        {
            get { return parentWidth; }
            set {
                parentWidth = value;
                this.VegasTrack.ParentTrackMotion.MotionKeyframes[0].Width = value;
            }
        }


        private double parentHeight;

        public double ParentHeight
        {
            get { return parentHeight; }
            set {
                parentHeight = value;
                this.VegasTrack.ParentTrackMotion.MotionKeyframes[0].Height = value;
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

        private VegPosition parentPosition;

        public VegPosition ParentPosition
        {
            get { return parentPosition; }
            set {
                parentPosition = value;
                this.VegasTrack.ParentTrackMotion.MotionKeyframes[0].PositionX = value.X;
                this.VegasTrack.ParentTrackMotion.MotionKeyframes[0].PositionY = value.Y;
            }
        }


        public override Effect maskEffect { get; set; }

        public VideoTrack VegasTrack { get; set; }

        public void SetSize(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }

        public void SetParentSize(double parentWidth, double parentHeight)
        {
            this.ParentWidth = parentWidth;
            this.ParentHeight = parentHeight;
        }
    }
}
