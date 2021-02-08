namespace Banana {
    public class Pt {
        private double x;
        private double y;
        private double d;

        public Pt(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public Pt(double x, double y, double d) {
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
        }

        public static Pt operator +(Pt v1, Pt v2) {
            return new Pt(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Pt operator -(Pt v1, Pt v2) {
            return new Pt(v1.X + -v2.X, v1.Y + -v2.Y);
        }

        public static Pt operator *(Pt v, double d) {
            return new Pt(v.X * d, v.Y * d);
        }


        public double Dot(Pt other) {
            return this.X * other.X + this.Y * other.Y;
        }

        public double Cross(Pt other) {
            return this.X * other.Y + -this.Y * other.X;
        }

        public static bool OnSeg(Pt p1, Pt p2, Pt q) {
            return (p1 - q).Cross(p2 - q) == 0 && (p1 - q).Dot(p2 - q) <= 0;
        }

        public static double OnSeg2(Pt p1, Pt p2, Pt q) {
            if ((p1 - q).Cross(p2 - q) == 0 && (p1 - q).Dot(p2 - q) <= 0) {
                // 線上の点だったとき
                return 0;
            } else {
                // 点が線のどっちにいるかは + と - で表します
                return (p1 - q).Cross(p2 - q);
            }
        }
    }
}
