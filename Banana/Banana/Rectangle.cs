namespace Banana {

    public class Rectangle {

        private readonly Pt p0 = null;
        private readonly Pt p1 = null;
        private readonly Pt p2 = null;
        private readonly Pt p3 = null;

        public Rectangle(Pt p0, Pt p1, Pt p2, Pt p3) {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }

        public Pt P0 {
            get => p0;
        }

        public Pt P1 {
            get => p1;
        }

        public Pt P2 {
            get => p2;
        }

        public Pt P3 {
            get => p3;
        }
    }
}
