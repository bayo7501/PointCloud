namespace Banana {

    public class Rectangle {

        private readonly XYDepth p0 = null;
        private readonly XYDepth p1 = null;
        private readonly XYDepth p2 = null;
        private readonly XYDepth p3 = null;

        public Rectangle(XYDepth p0, XYDepth p1, XYDepth p2, XYDepth p3) {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }

        public XYDepth P0 {
            get => p0;
        }

        public XYDepth P1 {
            get => p1;
        }

        public XYDepth P2 {
            get => p2;
        }

        public XYDepth P3 {
            get => p3;
        }
    }
}
