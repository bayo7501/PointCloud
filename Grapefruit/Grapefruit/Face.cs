using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grapefruit {
    public class Face {
        private Pnt a = null;
        private Pnt b = null;
        private Pnt c = null;

        public Face(Pnt a, Pnt b, Pnt c) {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public Pnt A {
            get => a;
        }

        public Pnt B {
            get => b;
        }

        public Pnt C {
            get => c;
        }
    }
}
