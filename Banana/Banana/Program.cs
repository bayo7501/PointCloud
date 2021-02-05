using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana {
    class Program {

        /// <summary>
        /// メッシュの長さ
        /// </summary>
        private static readonly int meshLength = 500; // 5000;
        /// <summary>
        /// 1目盛りのメッシュの大きさ
        /// </summary>
        private static readonly double meshPitch = 0.5;

        static void Main(string[] args) {

            Pt origin = new Pt(0, 0);

            List<Pt> pointsL = new List<Pt> {
                new Pt(10, 10),
                new Pt(10, 20),
                new Pt(10, 30),
                new Pt(10, 40),
                new Pt(10, 50)
            };

            List<Pt> pointsC = new List<Pt> {
                new Pt(20, 10),
                new Pt(20, 20),
                new Pt(20, 30),
                new Pt(20, 40),
                new Pt(20, 50)
            };

            List<Pt> pointsR = new List<Pt> {
                new Pt(30, 10),
                new Pt(30, 20),
                new Pt(30, 30),
                new Pt(30, 40),
                new Pt(30, 50)
            };

            Console.WriteLine(string.Empty);
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine("方向を決める");

            // 公共座標で扱っているので縦軸X、横軸Yで考える（数学軸なら縦軸Y、横軸X）
            double t = Math.Atan2(0 - origin.Y, 1 - origin.X);
            // 指定したモードの範囲内にラジアンを収める
            double d = Math2.Clamp(2, t);

            Console.WriteLine(string.Format("atan2 = {0}", t));
            Console.WriteLine(string.Format("ラジアンから度へ:{0}度", Math2.ToAngle(t)));
            Console.WriteLine(string.Format("正規化した値:{0}", d));
            Console.WriteLine(string.Format("ラジアンから度へ:{0}度", Math2.ToAngle(d)));

            Console.WriteLine(string.Empty);
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine("原点を点群パラメータに寄せたときの原点座標を求めます");

            double minX = double.NaN;
            double minY = double.NaN;

            double localX = 0.0D;
            double localY = 0.0D;

            bool b = false;

            for (int i = 0; i < 3; i++) {
                if (i == 0) 
                    b = Math2.ToLocal(pointsL[i].X, pointsL[i].Y, origin.X, origin.Y, -d, ref localX, ref localY);
                else if (i == 1)
                    b = Math2.ToLocal(pointsC[i].X, pointsC[i].Y, origin.X, origin.Y, -d, ref localX, ref localY);
                else if (i == 2)
                    b = Math2.ToLocal(pointsR[i].X, pointsR[i].Y, origin.X, origin.Y, -d, ref localX, ref localY);

                if (!b) return;

                if ((localX % meshPitch) != 0) {
                    localX -= (localX % meshPitch);
                    localX -= meshPitch;
                }

                if ((localY % meshPitch) != 0) {
                    localY -= (localY % meshPitch);
                    localY -= meshPitch;
                }

                if (localX < minX || double.IsNaN(minX))
                    minX = localX;

                if (localY < minY || double.IsNaN(minY))
                    minY = localY;
            }

            double originX = 0.0;
            double originY = 0.0;

            b = Math2.ToPublic(minX, minY, origin.X, origin.Y, -d, ref originX, ref originY);
            if (!b) return;

            Console.WriteLine(string.Format("原点座標 X:0 → {0}, Y:0 → {1}", originX, originY));


            Console.WriteLine(string.Empty);
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine(string.Format(
                "点群パラメータが {0}×{0} のメッシュに収まるかチェックします",
                meshLength));

            double maxX = pointsL.Max(a => a.X);
            if (maxX < pointsC.Max(a => a.X))
                maxX = pointsC.Max(a => a.X);
            if (maxX < pointsL.Max(a => a.X))
                maxX = pointsL.Max(a => a.X);

            double maxY = pointsL.Max(a => a.Y);
            if (maxX < pointsC.Max(a => a.Y))
                maxX = pointsC.Max(a => a.Y);
            if (maxX < pointsL.Max(a => a.Y))
                maxX = pointsL.Max(a => a.Y);

            Console.WriteLine(string.Format("点群パラメータのうち最も遠い点 X:{0} Y:{1}", maxX, maxY));

            Pt shiftOrigin = new Pt(originX, originY);

            // MeshLength * MeshLength に収まっているか
            if ((meshPitch + shiftOrigin.X) * meshLength < meshPitch * maxX)
                Console.WriteLine(string.Format("X範囲外:{0}", maxX));
            else
                Console.WriteLine("X 範囲内");

            if ((meshPitch + shiftOrigin.Y) * meshLength < meshPitch * maxY)
                Console.WriteLine(string.Format("Y範囲外:{0}", maxY));
            else
                Console.WriteLine("Y 範囲内");


        }
    }
}
