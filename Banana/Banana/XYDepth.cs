namespace Banana {
    public class XYDepth {
        private double x;
        private double y;
        private double d;

        public XYDepth(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public XYDepth(double x, double y, double d) {
            this.x = x;
            this.y = y;
            this.d = d;
        }

        public double X {
            get => x;
        }
        public double Y {
            get => y;
        }
        public double D {
            get => d;
            set => d = value;
        }

    }
}
