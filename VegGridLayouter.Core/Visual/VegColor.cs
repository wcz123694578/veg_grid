using System;

namespace VegGridLayouter.Core.Visual
{
    public class VegColor : VegBrush
    {
        public VegColor()
        {

        }

        public VegColor(int _R, int _G, int _B, int _A)
        {
            this.R = _R;
            this.G = _G;
            this.B = _B;
            this.A = _A;
        }

        private int r;

        public int R
        {
            get { return r; }
            set {
                if (value < 0 || value > 255)
                {
                    value = 0;
                }
                r = value;
            }
        }

        private int g;

        public int G
        {
            get { return g; }
            set {
                if (value < 0 || value > 255)
                {
                    value = 0;
                }
                g = value;
            }
        }

        private int b;

        public int B
        {
            get { return b; }
            set
            {
                if (value < 0 || value > 255)
                {
                    value = 0;
                }
                b = value;
            }
        }

        private double a;

        public double A
        {
            get { return a; }
            set {
                if (value < 0 || value > 1)
                {
                    value = 1;
                }
                a = value;
            }
        }

        public override string ToString()
        {
            return $"R = {this.R}\nG = {this.G}\nB = {this.B}\nAlpha = {this.A}\n";
        }
    }
}
