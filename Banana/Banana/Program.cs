using System;
using System.Collections.Generic;
using System.Linq;

namespace Banana {
    class Program {

        /// <summary>
        /// メッシュの長さ(単位:m)
        /// </summary>
        private static readonly int meshLength = 120;
        /// <summary>
        /// 1目盛りのメッシュの大きさ(単位:m)
        /// </summary>
        private static readonly double meshPitch = 0.5;

        static void Main(string[] args) {

            Logger logger = new Logger(Logger.P);
            //Logger.Write(Logger.E, "E");
            //Logger.Write(Logger.W, "W");
            //Logger.Write(Logger.I, "I");
            //Logger.Write(Logger.D, "D");
            //Logger.Write(Logger.P, "P");

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            Logger.Write(Logger.P, "START");

            XYDepth origin = new XYDepth(0, 0);
            // L座標 公共XY(単位:m)
            List<XYDepth> pointsL = new List<XYDepth> {
                new XYDepth(10, 10,  5),
                new XYDepth(20, 10, 10),
                new XYDepth(30, 10, 15),
                new XYDepth(40, 10, 20),
                new XYDepth(50, 10, 25)
            };

            // C座標 公共XY(単位:m)
            List<XYDepth> pointsC = new List<XYDepth> {
                new XYDepth(10, 20, 25),
                new XYDepth(20, 20, 20),
                new XYDepth(30, 20, 15),
                new XYDepth(40, 20, 10),
                new XYDepth(50, 20, 5)
            };

            // R座標 公共XY(単位:m)
            List<XYDepth> pointsR = new List<XYDepth> {
                new XYDepth(10, 30, 10),
                new XYDepth(20, 30, 20),
                new XYDepth(30, 30, 30),
                new XYDepth(40, 30, 40),
                new XYDepth(50, 30, 50)
            };

            Logger.Write(Logger.D, "---------------------------------------------------------------------------");
            Logger.Write(Logger.D, "方向を決める");

            // 公共座標で扱っているので縦軸X、横軸Yで考える（数学軸なら縦軸Y、横軸X）
            double t = Math.Atan2(0 - origin.Y, 1 - origin.X);
            // 指定したモードの範囲内にラジアンを収める
            double d = Math2.Clamp(2, t);

            Logger.Write(Logger.D, $"atan2 = {t}");
            Logger.Write(Logger.D, $"ラジアンから度へ:{Math2.ToAngle(t)}度");
            Logger.Write(Logger.D, $"正規化した値:{d}");
            Logger.Write(Logger.D, $"ラジアンから度へ:{Math2.ToAngle(d)}度");

            Logger.Write(Logger.D, "---------------------------------------------------------------------------");
            Logger.Write(Logger.D, "点群パラメータに最も近い位置に原点を寄せたときの原点座標を求めます");

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

            XYDepth shiftOrigin = new XYDepth(originX, originY);
            Logger.Write(Logger.D, $"原点座標 X:0 → {shiftOrigin.X}, Y:0 → {shiftOrigin.Y}");


            Logger.Write(Logger.D, "---------------------------------------------------------------------------");
            Logger.Write(Logger.D, $"点群パラメータが {meshLength}×{meshLength} のメッシュに収まるかチェックします");
            Logger.Write(Logger.D, $"メートル換算で {meshLength * meshPitch}m × {meshLength * meshPitch}m");

            double maxX = pointsL.Max(a => a.X);
            if (maxX < pointsC.Max(a => a.X))
                maxX = pointsC.Max(a => a.X);
            if (maxX < pointsR.Max(a => a.X))
                maxX = pointsR.Max(a => a.X);

            double maxY = pointsL.Max(a => a.Y);
            if (maxY < pointsC.Max(a => a.Y))
                maxY = pointsC.Max(a => a.Y);
            if (maxY < pointsR.Max(a => a.Y))
                maxY = pointsR.Max(a => a.Y);

            Logger.Write(Logger.D, $"点群パラメータのうち最も遠い点 X:{maxX} Y:{maxY}");

            // MeshLength * MeshLength に収まっているか
            // 最も遠い公共座標(単位:m)をマス目換算して 50 / 0.5 = 100
            // シフト原点もマス目換算したもので引き算。
            // 使用するマス目がメッシュサイズの上限を超えていないかチェック
            if (meshLength < maxX / meshPitch - shiftOrigin.X / meshPitch)
                Logger.Write(Logger.D, $"X 範囲外:{maxX}");
            else
                Logger.Write(Logger.D, "X 範囲内");

            if (meshLength < maxY / meshPitch - shiftOrigin.Y / meshPitch)
                Logger.Write(Logger.D, $"Y 範囲外:{maxY}");
            else
                Logger.Write(Logger.D, "Y 範囲内");

            DepthArray depthArray = new DepthArray(meshLength, pointsL, pointsC, pointsR, shiftOrigin, meshPitch);
            depthArray.Create();

            sw.Stop();
            Logger.Write(Logger.P, "END");

            Logger.Write(Logger.P, "■処理にかかった時間");
            TimeSpan ts = sw.Elapsed;
            Logger.Write(Logger.P, $"　{ts}");
            Logger.Write(Logger.P, $"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
            Logger.Write(Logger.P, $"　{sw.ElapsedMilliseconds}ミリ秒");

            int length = depthArray.Depths.Length;
            double seekStartX = 0.25 + shiftOrigin.X;
            double seekStartY = 0.25 + shiftOrigin.Y;

            for (int i = 0; i < length; i++) {
                int y = Math.DivRem(i, meshLength, out int x);
                Logger.Write(Logger.P, $"X:{ seekStartX + x * meshPitch } Y:{seekStartY + y * meshPitch} Depth:{depthArray.Depths[i] / 10.0}");
            }

            Console.ReadLine();
        }
    }
}
