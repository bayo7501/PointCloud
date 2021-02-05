namespace Apple {
    public class XY {
        private readonly double x = double.NaN;
        private readonly double y = double.NaN;
        public XY(double x, double y) {
            this.x = x;
            this.y = y;
        }
        public double X {
            get => x;
        }
        public double Y {
            get => y;
        }
    }
}
