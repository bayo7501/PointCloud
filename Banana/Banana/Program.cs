using System;
using System.Collections.Generic;
using System.Linq;

namespace Banana {
    class Program {

        /// <summary>
        /// メッシュの長さ(単位:m)
        /// </summary>
        private static readonly int meshLength = 5000;
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
                new XYDepth( 0, 10,  5),
                new XYDepth( 1, 10, 10),
                new XYDepth( 2, 10, 15),
                new XYDepth( 3, 10, 20),
                new XYDepth( 4, 10, 25),
                new XYDepth( 5, 10,  5),
                new XYDepth( 6, 10, 10),
                new XYDepth( 7, 10, 15),
                new XYDepth( 8, 10, 20),
                new XYDepth( 9, 10, 25),
                new XYDepth(10, 10,  5),
                new XYDepth(11, 10, 10),
                new XYDepth(12, 10, 15),
                new XYDepth(13, 10, 20),
                new XYDepth(14, 10, 25),
                new XYDepth(15, 10,  5),
                new XYDepth(16, 10, 10),
                new XYDepth(17, 10, 15),
                new XYDepth(18, 10, 20),
                new XYDepth(19, 10, 25),
                new XYDepth(20, 10,  5),
                new XYDepth(21, 10, 10),
                new XYDepth(22, 10, 15),
                new XYDepth(23, 10, 20),
                new XYDepth(24, 10, 25),
                new XYDepth(25, 10,  5),
                new XYDepth(26, 10, 10),
                new XYDepth(27, 10, 15),
                new XYDepth(28, 10, 20),
                new XYDepth(29, 10, 25),
                new XYDepth(30, 10,  5),
                new XYDepth(31, 10, 10),
                new XYDepth(32, 10, 15),
                new XYDepth(33, 10, 20),
                new XYDepth(34, 10, 25),
                new XYDepth(35, 10,  5),
                new XYDepth(36, 10, 10),
                new XYDepth(37, 10, 15),
                new XYDepth(38, 10, 20),
                new XYDepth(39, 10, 25),
                new XYDepth(40, 10,  5),
                new XYDepth(41, 10, 10),
                new XYDepth(42, 10, 15),
                new XYDepth(43, 10, 20),
                new XYDepth(44, 10, 25),
                new XYDepth(45, 10,  5),
                new XYDepth(46, 10, 10),
                new XYDepth(47, 10, 15),
                new XYDepth(48, 10, 20),
                new XYDepth(49, 10, 25),
                new XYDepth(50, 10,  5),
                new XYDepth(51, 10, 10),
                new XYDepth(52, 10, 15),
                new XYDepth(53, 10, 20),
                new XYDepth(54, 10, 25),
                new XYDepth(55, 10,  5),
                new XYDepth(56, 10, 10),
                new XYDepth(57, 10, 15),
                new XYDepth(58, 10, 20),
                new XYDepth(59, 10, 25),
                new XYDepth(60, 10,  5),
                new XYDepth(61, 10, 10),
                new XYDepth(62, 10, 15),
                new XYDepth(63, 10, 20),
                new XYDepth(64, 10, 25),
                new XYDepth(65, 10,  5),
                new XYDepth(66, 10, 10),
                new XYDepth(67, 10, 15),
                new XYDepth(68, 10, 20),
                new XYDepth(69, 10, 25),
                new XYDepth(70, 10,  5),
                new XYDepth(71, 10, 10),
                new XYDepth(72, 10, 15),
                new XYDepth(73, 10, 20),
                new XYDepth(74, 10, 25),
                new XYDepth(75, 10,  5),
                new XYDepth(76, 10, 10),
                new XYDepth(77, 10, 15),
                new XYDepth(78, 10, 20),
                new XYDepth(79, 10, 25),
                new XYDepth(80, 10,  5),
                new XYDepth(81, 10, 10),
                new XYDepth(82, 10, 15),
                new XYDepth(83, 10, 20),
                new XYDepth(84, 10, 25),
                new XYDepth(85, 10,  5),
                new XYDepth(86, 10, 10),
                new XYDepth(87, 10, 15),
                new XYDepth(88, 10, 20),
                new XYDepth(89, 10, 25),
                new XYDepth(90, 10,  5),
                new XYDepth(91, 10, 10),
                new XYDepth(92, 10, 15),
                new XYDepth(93, 10, 20),
                new XYDepth(94, 10, 25),
                new XYDepth(95, 10,  5),
                new XYDepth(96, 10, 10),
                new XYDepth(97, 10, 15),
                new XYDepth(98, 10, 20),
                new XYDepth(99, 10, 25),
            };

            // C座標 公共XY(単位:m)
            List<XYDepth> pointsC = new List<XYDepth> {
                new XYDepth( 0, 20, 25),
                new XYDepth( 1, 20, 20),
                new XYDepth( 2, 20, 15),
                new XYDepth( 3, 20, 10),
                new XYDepth( 4, 20,  5),
                new XYDepth( 5, 20, 25),
                new XYDepth( 6, 20, 20),
                new XYDepth( 7, 20, 15),
                new XYDepth( 8, 20, 10),
                new XYDepth( 9, 20,  5),
                new XYDepth(10, 20, 25),
                new XYDepth(11, 20, 20),
                new XYDepth(12, 20, 15),
                new XYDepth(13, 20, 10),
                new XYDepth(14, 20,  5),
                new XYDepth(15, 20, 25),
                new XYDepth(16, 20, 20),
                new XYDepth(17, 20, 15),
                new XYDepth(18, 20, 10),
                new XYDepth(19, 20,  5),
                new XYDepth(20, 20, 25),
                new XYDepth(21, 20, 20),
                new XYDepth(22, 20, 15),
                new XYDepth(23, 20, 10),
                new XYDepth(24, 20,  5),
                new XYDepth(25, 20, 25),
                new XYDepth(26, 20, 20),
                new XYDepth(27, 20, 15),
                new XYDepth(28, 20, 10),
                new XYDepth(29, 20,  5),
                new XYDepth(30, 20, 25),
                new XYDepth(31, 20, 20),
                new XYDepth(32, 20, 15),
                new XYDepth(33, 20, 10),
                new XYDepth(34, 20,  5),
                new XYDepth(35, 20, 25),
                new XYDepth(36, 20, 20),
                new XYDepth(37, 20, 15),
                new XYDepth(38, 20, 10),
                new XYDepth(39, 20,  5),
                new XYDepth(40, 20, 25),
                new XYDepth(41, 20, 20),
                new XYDepth(42, 20, 15),
                new XYDepth(43, 20, 10),
                new XYDepth(44, 20,  5),
                new XYDepth(45, 20, 25),
                new XYDepth(46, 20, 20),
                new XYDepth(47, 20, 15),
                new XYDepth(48, 20, 10),
                new XYDepth(49, 20,  5),
                new XYDepth(50, 20, 25),
                new XYDepth(51, 20, 20),
                new XYDepth(52, 20, 15),
                new XYDepth(53, 20, 10),
                new XYDepth(54, 20,  5),
                new XYDepth(55, 20, 25),
                new XYDepth(56, 20, 20),
                new XYDepth(57, 20, 15),
                new XYDepth(58, 20, 10),
                new XYDepth(59, 20,  5),
                new XYDepth(60, 20, 25),
                new XYDepth(61, 20, 20),
                new XYDepth(62, 20, 15),
                new XYDepth(63, 20, 10),
                new XYDepth(64, 20,  5),
                new XYDepth(65, 20, 25),
                new XYDepth(66, 20, 20),
                new XYDepth(67, 20, 15),
                new XYDepth(68, 20, 10),
                new XYDepth(69, 20,  5),
                new XYDepth(70, 20, 25),
                new XYDepth(71, 20, 20),
                new XYDepth(72, 20, 15),
                new XYDepth(73, 20, 10),
                new XYDepth(74, 20,  5),
                new XYDepth(75, 20, 25),
                new XYDepth(76, 20, 20),
                new XYDepth(77, 20, 15),
                new XYDepth(78, 20, 10),
                new XYDepth(79, 20,  5),
                new XYDepth(80, 20, 25),
                new XYDepth(81, 20, 20),
                new XYDepth(82, 20, 15),
                new XYDepth(83, 20, 10),
                new XYDepth(84, 20,  5),
                new XYDepth(85, 20, 25),
                new XYDepth(86, 20, 20),
                new XYDepth(87, 20, 15),
                new XYDepth(88, 20, 10),
                new XYDepth(89, 20,  5),
                new XYDepth(90, 20, 25),
                new XYDepth(91, 20, 20),
                new XYDepth(92, 20, 15),
                new XYDepth(93, 20, 10),
                new XYDepth(94, 20,  5),
                new XYDepth(95, 20, 25),
                new XYDepth(96, 20, 20),
                new XYDepth(97, 20, 15),
                new XYDepth(98, 20, 10),
                new XYDepth(99, 20,  5),
            };

            // R座標 公共XY(単位:m)
            List<XYDepth> pointsR = new List<XYDepth> {
                new XYDepth( 0, 30, 10),
                new XYDepth( 1, 30, 20),
                new XYDepth( 2, 30, 30),
                new XYDepth( 3, 30, 40),
                new XYDepth( 4, 30, 50),
                new XYDepth( 5, 30, 10),
                new XYDepth( 6, 30, 20),
                new XYDepth( 7, 30, 30),
                new XYDepth( 8, 30, 40),
                new XYDepth( 9, 30, 50),
                new XYDepth(10, 30, 10),
                new XYDepth(11, 30, 20),
                new XYDepth(12, 30, 30),
                new XYDepth(13, 30, 40),
                new XYDepth(14, 30, 50),
                new XYDepth(15, 30, 10),
                new XYDepth(16, 30, 20),
                new XYDepth(17, 30, 30),
                new XYDepth(18, 30, 40),
                new XYDepth(19, 30, 50),
                new XYDepth(20, 30, 10),
                new XYDepth(21, 30, 20),
                new XYDepth(22, 30, 30),
                new XYDepth(23, 30, 40),
                new XYDepth(24, 30, 50),
                new XYDepth(25, 30, 10),
                new XYDepth(26, 30, 20),
                new XYDepth(27, 30, 30),
                new XYDepth(28, 30, 40),
                new XYDepth(29, 30, 50),
                new XYDepth(30, 30, 10),
                new XYDepth(31, 30, 20),
                new XYDepth(32, 30, 30),
                new XYDepth(33, 30, 40),
                new XYDepth(34, 30, 50),
                new XYDepth(35, 30, 10),
                new XYDepth(36, 30, 20),
                new XYDepth(37, 30, 30),
                new XYDepth(38, 30, 40),
                new XYDepth(39, 30, 50),
                new XYDepth(40, 30, 10),
                new XYDepth(41, 30, 20),
                new XYDepth(42, 30, 30),
                new XYDepth(43, 30, 40),
                new XYDepth(44, 30, 50),
                new XYDepth(45, 30, 10),
                new XYDepth(46, 30, 20),
                new XYDepth(47, 30, 30),
                new XYDepth(48, 30, 40),
                new XYDepth(49, 30, 50),
                new XYDepth(50, 30, 10),
                new XYDepth(51, 30, 20),
                new XYDepth(52, 30, 30),
                new XYDepth(53, 30, 40),
                new XYDepth(54, 30, 50),
                new XYDepth(55, 30, 10),
                new XYDepth(56, 30, 20),
                new XYDepth(57, 30, 30),
                new XYDepth(58, 30, 40),
                new XYDepth(59, 30, 50),
                new XYDepth(60, 30, 10),
                new XYDepth(61, 30, 20),
                new XYDepth(62, 30, 30),
                new XYDepth(63, 30, 40),
                new XYDepth(64, 30, 50),
                new XYDepth(65, 30, 10),
                new XYDepth(66, 30, 20),
                new XYDepth(67, 30, 30),
                new XYDepth(68, 30, 40),
                new XYDepth(69, 30, 50),
                new XYDepth(70, 30, 10),
                new XYDepth(71, 30, 20),
                new XYDepth(72, 30, 30),
                new XYDepth(73, 30, 40),
                new XYDepth(74, 30, 50),
                new XYDepth(75, 30, 10),
                new XYDepth(76, 30, 20),
                new XYDepth(77, 30, 30),
                new XYDepth(78, 30, 40),
                new XYDepth(79, 30, 50),
                new XYDepth(80, 30, 10),
                new XYDepth(81, 30, 20),
                new XYDepth(82, 30, 30),
                new XYDepth(83, 30, 40),
                new XYDepth(84, 30, 50),
                new XYDepth(85, 30, 10),
                new XYDepth(86, 30, 20),
                new XYDepth(87, 30, 30),
                new XYDepth(88, 30, 40),
                new XYDepth(89, 30, 50),
                new XYDepth(90, 30, 10),
                new XYDepth(91, 30, 20),
                new XYDepth(92, 30, 30),
                new XYDepth(93, 30, 40),
                new XYDepth(94, 30, 50),
                new XYDepth(95, 30, 10),
                new XYDepth(96, 30, 20),
                new XYDepth(97, 30, 30),
                new XYDepth(98, 30, 40),
                new XYDepth(99, 30, 50),
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

                if (i == 8000)
                    break;
            }

            Console.ReadLine();
        }
    }
}
