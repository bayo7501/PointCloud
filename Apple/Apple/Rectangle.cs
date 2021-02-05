namespace Apple {
    public class Rectangle {
        private readonly XY p0 = null;
        private readonly XY p1 = null;
        private readonly XY p2 = null;
        private readonly XY p3 = null;
        public Rectangle(XY p0, XY p1, XY p2, XY p3) {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }
        public XY P0 {
            get => p0;
        }
        public XY P1 {
            get => p1;
        }
        public XY P2 {
            get => p2;
        }
        public XY P3 {
            get => p3;
        }
    }
}
