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

        public static XYDepth operator +(XYDepth v1, XYDepth v2) {
            return new XYDepth(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static XYDepth operator -(XYDepth v1, XYDepth v2) {
            return new XYDepth(v1.X + -v2.X, v1.Y + -v2.Y);
        }

        public static XYDepth operator *(XYDepth v, double d) {
            return new XYDepth(v.X * d, v.Y * d);
        }


        /// <summary>
        /// 内積
        /// Dot product of two vectors: O -> this and O -> other
        ///  a dot b = |a| |b| cos(theta) = ax bx + ax by
        ///  zero if two vectors run orthogonally
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double Dot(XYDepth other) {
            return this.X * other.X + this.Y * other.Y;
        }

        /// <summary>
        /// 外積
        /// Cross(det) product of two vectors: O -> this and O -> other
        ///  a x b = |a| |b| sin(theta) = ax by - ay bx
        ///  zero if two vectors run parallelly
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double Cross(XYDepth other) {
            return this.X * other.Y + -this.Y * other.X;
        }

        /// <summary>
        /// 点p1 - 点p2 を結ぶ線分の上に 点q があるか?
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static bool OnSeg(XYDepth p1, XYDepth p2, XYDepth q) {
            return (p1 - q).Cross(p2 - q) == 0 && (p1 - q).Dot(p2 - q) <= 0;
        }

        /// <summary>
        /// 点p1 - 点p2 を結ぶ線分の左右どちらに 点q があるか?
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static double OnSeg2(XYDepth p1, XYDepth p2, XYDepth q) {
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
