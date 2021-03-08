using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grapefruit {
    public class Pnt {
        private string id = string.Empty;
        private double x = double.NaN;
        private double y = double.NaN;
        private double d = double.NaN;

        public Pnt(string id, double x, double y, double d) {
            this.id = id;
            this.x = x;
            this.y = y;
            this.d = d;
        }

        public Pnt(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public string ID {
            get => id;
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
    }
}
