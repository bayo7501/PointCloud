using System;

namespace Apple {
    public class DoubleUtil {
        private static double EPS = 1e-10;

        public static double Add(double a, double b) {
            if (Math.Abs(a + b) < EPS * (Math.Abs(a) + Math.Abs(b)))
                return 0;
            return a + b;
        }

        public static bool Eq(double a, double b) {
            return Math.Abs(a - b) < 1e-9;
        }
    }
}
