using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana {
    public class Depth {

        private List<Pt> pointsL;
        private List<Pt> pointsL;
        private List<Pt> pointsL;
        public Depth() {

        }

        public void Init() {
            Console.WriteLine(string.Empty);
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine("点群パラメータから小さい四角形を作ります");

            List<Rectangle> rectangles = new List<Rectangle>();
            // L列の点とC列の点で構成する四角形を作る
            for (int i = 0; i < pointsL.Count; i++) {
                if (i == pointsL.Count - 1)
                    break;
                Rectangle rectangle = new Rectangle(pointsL[i], pointsL[i + 1], pointsC[i + 1], pointsC[i]);
                rectangles.Add(rectangle);
            }
            Console.WriteLine(string.Format("四角形の数:{0}", rectangles.Count));
            // C列の点とR列の点で構成する四角形を作る
            for (int i = 0; i < pointsC.Count; i++) {
                if (i == pointsC.Count - 1)
                    break;
                Rectangle rectangle = new Rectangle(pointsC[i], pointsC[i + 1], pointsR[i + 1], pointsR[i]);
                rectangles.Add(rectangle);
            }
            Console.WriteLine(string.Format("四角形の数:{0}", rectangles.Count));

        }
    }
}
