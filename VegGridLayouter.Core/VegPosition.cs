namespace VegGridLayouter.Core
{
    public class VegPosition
    {
        public VegPosition()
        {

        }

        public VegPosition(double _X, double _Y)
        {
            this.X = _X;
            this.Y = _Y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public override string ToString()
        {
            return $"X: {this.X}\nY: {this.Y}\n";
        }
    }

    public class VegPosition3D : VegPosition
    {
        public double Z { get; set; }
    }
}
