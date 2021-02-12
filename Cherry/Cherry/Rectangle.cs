namespace Cherry {

    public class Rectangle {

        /*
         *    B  +---+---+---+  C
         *       |           |
         *       +           +
         *       |           |
         *       +           +
         *       |           |
         *    A  +---+---+---+  D
         */

        private readonly XYD a = null;
        private readonly XYD b = null;
        private readonly XYD c = null;
        private readonly XYD d = null;

        public Rectangle(XYD a, XYD b, XYD c, XYD d) {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public XYD A {
            get => a;
        }

        public XYD B {
            get => b;
        }

        public XYD C {
            get => c;
        }

        public XYD D {
            get => d;
        }
    }
}
