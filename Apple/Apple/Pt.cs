using System;

namespace Apple {
    public class Pt {
        public double X;
        public double Y;

        public Pt(double x, double y) {
            X = x;
            Y = y;
        }

        public static Pt operator +(Pt v1, Pt v2) {
            return new Pt(DoubleUtil.Add(v1.X, v2.X), DoubleUtil.Add(v1.Y, v2.Y));
        }
        public static Pt operator -(Pt v1, Pt v2) {
            return new Pt(DoubleUtil.Add(v1.X, -v2.X), DoubleUtil.Add(v1.Y, -v2.Y));
        }
        public static Pt operator *(Pt v, double d) {
            return new Pt(v.X * d, v.Y * d);
        }

        /// <summary>
        /// Dot product of two vectors: O -> this and O -> other
        ///  a dot b = |a| |b| cos(theta) = ax bx + ax by
        ///  zero if two vectors run orthogonally
        /// </summary>
        public double Dot(Pt other) {
            return DoubleUtil.Add(this.X * other.X, this.Y * other.Y);
        }

        /// <summary>
        /// Cross(det) product of two vectors: O -> this and O -> other
        ///  a x b = |a| |b| sin(theta) = ax by - ay bx
        ///  zero if two vectors run parallelly
        /// </summary>
        public double Cross(Pt other) {
            return DoubleUtil.Add(this.X * other.Y, -this.Y * other.X);
        }

        /// <summary>
        /// point q exists on line p1-p2?
        /// </summary>
        public static bool OnSeg(Pt p1, Pt p2, Pt q) {
            return (p1 - q).Cross(p2 - q) == 0 && (p1 - q).Dot(p2 - q) <= 0;
        }
        public static double OnSeg2(Pt p1, Pt p2, Pt q) {

            if ((p1 - q).Cross(p2 - q) == 0 && (p1 - q).Dot(p2 - q) <= 0) {
                return 0;
            } else {
                return (p1 - q).Cross(p2 - q);
            }

        }

        /// <summary>
        /// crosssing point of line p1-p2 and q1-q2
        /// </summary>
        public static Pt Intersect(Pt p1, Pt p2, Pt q1, Pt q2) {
            return p1 + (p2 - p1) * ((q2 - q1).Cross(q1 - p1) / (q2 - q1).Cross(p2 - p1));
        }
        public static bool HasIntersect(Pt p1, Pt p2, Pt q1, Pt q2) {
            if ((p1 - q1).Cross(p2 - q2) == 0) {
                return OnSeg(p1, q1, p2) || OnSeg(p1, q1, q2) || OnSeg(p2, q2, p1) || OnSeg(p2, q2, q1);
            } else {
                var r = Intersect(p1, q1, p2, q2);
                return OnSeg(p1, q1, r) && OnSeg(p2, q2, r);
            }
        }

        public static bool operator ==(Pt x, Pt y) {
            return (DoubleUtil.Eq(x.X, y.X) && DoubleUtil.Eq(x.Y, y.Y));
        }

        public static bool operator !=(Pt x, Pt y) {
            return (!DoubleUtil.Eq(x.X, y.X) || !DoubleUtil.Eq(x.Y, y.Y));
        }
        public double Norm() {
            return Math.Sqrt(X * X + Y * Y);
        }
        public double Dist(Pt other) {
            return (this - other).Norm();
        }
        public static double Dist(Pt v1, Pt v2) {
            return v1.Dist(v2);
        }
    }
}
