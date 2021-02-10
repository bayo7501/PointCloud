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

            Logger logger = new Logger(Logger.V);

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            Logger.Write(Logger.V, "START");

            XYD origin = new XYD(0, 0);
            // L座標 公共XY(単位:m)
            List<XYD> pointsL = new List<XYD> {
                new XYD( 0, 10,  5),
                new XYD( 1, 10, 10),
                new XYD( 2, 10, 15),
                new XYD( 3, 10, 20),
                new XYD( 4, 10, 25),
                new XYD( 5, 10,  5),
                new XYD( 6, 10, 10),
                new XYD( 7, 10, 15),
                new XYD( 8, 10, 20),
                new XYD( 9, 10, 25),
                new XYD(10, 10,  5),
                new XYD(11, 10, 10),
                new XYD(12, 10, 15),
                new XYD(13, 10, 20),
                new XYD(14, 10, 25),
                new XYD(15, 10,  5),
                new XYD(16, 10, 10),
                new XYD(17, 10, 15),
                new XYD(18, 10, 20),
                new XYD(19, 10, 25),
                new XYD(20, 10,  5),
                new XYD(21, 10, 10),
                new XYD(22, 10, 15),
                new XYD(23, 10, 20),
                new XYD(24, 10, 25),
                new XYD(25, 10,  5),
                new XYD(26, 10, 10),
                new XYD(27, 10, 15),
                new XYD(28, 10, 20),
                new XYD(29, 10, 25),
                new XYD(30, 10,  5),
                new XYD(31, 10, 10),
                new XYD(32, 10, 15),
                new XYD(33, 10, 20),
                new XYD(34, 10, 25),
                new XYD(35, 10,  5),
                new XYD(36, 10, 10),
                new XYD(37, 10, 15),
                new XYD(38, 10, 20),
                new XYD(39, 10, 25),
                new XYD(40, 10,  5),
                new XYD(41, 10, 10),
                new XYD(42, 10, 15),
                new XYD(43, 10, 20),
                new XYD(44, 10, 25),
                new XYD(45, 10,  5),
                new XYD(46, 10, 10),
                new XYD(47, 10, 15),
                new XYD(48, 10, 20),
                new XYD(49, 10, 25),
                new XYD(50, 10,  5),
                new XYD(51, 10, 10),
                new XYD(52, 10, 15),
                new XYD(53, 10, 20),
                new XYD(54, 10, 25),
                new XYD(55, 10,  5),
                new XYD(56, 10, 10),
                new XYD(57, 10, 15),
                new XYD(58, 10, 20),
                new XYD(59, 10, 25),
                new XYD(60, 10,  5),
                new XYD(61, 10, 10),
                new XYD(62, 10, 15),
                new XYD(63, 10, 20),
                new XYD(64, 10, 25),
                new XYD(65, 10,  5),
                new XYD(66, 10, 10),
                new XYD(67, 10, 15),
                new XYD(68, 10, 20),
                new XYD(69, 10, 25),
                new XYD(70, 10,  5),
                new XYD(71, 10, 10),
                new XYD(72, 10, 15),
                new XYD(73, 10, 20),
                new XYD(74, 10, 25),
                new XYD(75, 10,  5),
                new XYD(76, 10, 10),
                new XYD(77, 10, 15),
                new XYD(78, 10, 20),
                new XYD(79, 10, 25),
                new XYD(80, 10,  5),
                new XYD(81, 10, 10),
                new XYD(82, 10, 15),
                new XYD(83, 10, 20),
                new XYD(84, 10, 25),
                new XYD(85, 10,  5),
                new XYD(86, 10, 10),
                new XYD(87, 10, 15),
                new XYD(88, 10, 20),
                new XYD(89, 10, 25),
                new XYD(90, 10,  5),
                new XYD(91, 10, 10),
                new XYD(92, 10, 15),
                new XYD(93, 10, 20),
                new XYD(94, 10, 25),
                new XYD(95, 10,  5),
                new XYD(96, 10, 10),
                new XYD(97, 10, 15),
                new XYD(98, 10, 20),
                new XYD(99, 10, 25),
            };

            // C座標 公共XY(単位:m)
            List<XYD> pointsC = new List<XYD> {
                new XYD( 0, 20, 25),
                new XYD( 1, 20, 20),
                new XYD( 2, 20, 15),
                new XYD( 3, 20, 10),
                new XYD( 4, 20,  5),
                new XYD( 5, 20, 25),
                new XYD( 6, 20, 20),
                new XYD( 7, 20, 15),
                new XYD( 8, 20, 10),
                new XYD( 9, 20,  5),
                new XYD(10, 20, 25),
                new XYD(11, 20, 20),
                new XYD(12, 20, 15),
                new XYD(13, 20, 10),
                new XYD(14, 20,  5),
                new XYD(15, 20, 25),
                new XYD(16, 20, 20),
                new XYD(17, 20, 15),
                new XYD(18, 20, 10),
                new XYD(19, 20,  5),
                new XYD(20, 20, 25),
                new XYD(21, 20, 20),
                new XYD(22, 20, 15),
                new XYD(23, 20, 10),
                new XYD(24, 20,  5),
                new XYD(25, 20, 25),
                new XYD(26, 20, 20),
                new XYD(27, 20, 15),
                new XYD(28, 20, 10),
                new XYD(29, 20,  5),
                new XYD(30, 20, 25),
                new XYD(31, 20, 20),
                new XYD(32, 20, 15),
                new XYD(33, 20, 10),
                new XYD(34, 20,  5),
                new XYD(35, 20, 25),
                new XYD(36, 20, 20),
                new XYD(37, 20, 15),
                new XYD(38, 20, 10),
                new XYD(39, 20,  5),
                new XYD(40, 20, 25),
                new XYD(41, 20, 20),
                new XYD(42, 20, 15),
                new XYD(43, 20, 10),
                new XYD(44, 20,  5),
                new XYD(45, 20, 25),
                new XYD(46, 20, 20),
                new XYD(47, 20, 15),
                new XYD(48, 20, 10),
                new XYD(49, 20,  5),
                new XYD(50, 20, 25),
                new XYD(51, 20, 20),
                new XYD(52, 20, 15),
                new XYD(53, 20, 10),
                new XYD(54, 20,  5),
                new XYD(55, 20, 25),
                new XYD(56, 20, 20),
                new XYD(57, 20, 15),
                new XYD(58, 20, 10),
                new XYD(59, 20,  5),
                new XYD(60, 20, 25),
                new XYD(61, 20, 20),
                new XYD(62, 20, 15),
                new XYD(63, 20, 10),
                new XYD(64, 20,  5),
                new XYD(65, 20, 25),
                new XYD(66, 20, 20),
                new XYD(67, 20, 15),
                new XYD(68, 20, 10),
                new XYD(69, 20,  5),
                new XYD(70, 20, 25),
                new XYD(71, 20, 20),
                new XYD(72, 20, 15),
                new XYD(73, 20, 10),
                new XYD(74, 20,  5),
                new XYD(75, 20, 25),
                new XYD(76, 20, 20),
                new XYD(77, 20, 15),
                new XYD(78, 20, 10),
                new XYD(79, 20,  5),
                new XYD(80, 20, 25),
                new XYD(81, 20, 20),
                new XYD(82, 20, 15),
                new XYD(83, 20, 10),
                new XYD(84, 20,  5),
                new XYD(85, 20, 25),
                new XYD(86, 20, 20),
                new XYD(87, 20, 15),
                new XYD(88, 20, 10),
                new XYD(89, 20,  5),
                new XYD(90, 20, 25),
                new XYD(91, 20, 20),
                new XYD(92, 20, 15),
                new XYD(93, 20, 10),
                new XYD(94, 20,  5),
                new XYD(95, 20, 25),
                new XYD(96, 20, 20),
                new XYD(97, 20, 15),
                new XYD(98, 20, 10),
                new XYD(99, 20,  5),
            };

            // R座標 公共XY(単位:m)
            List<XYD> pointsR = new List<XYD> {
                new XYD( 0, 30, 10),
                new XYD( 1, 30, 20),
                new XYD( 2, 30, 30),
                new XYD( 3, 30, 40),
                new XYD( 4, 30, 50),
                new XYD( 5, 30, 10),
                new XYD( 6, 30, 20),
                new XYD( 7, 30, 30),
                new XYD( 8, 30, 40),
                new XYD( 9, 30, 50),
                new XYD(10, 30, 10),
                new XYD(11, 30, 20),
                new XYD(12, 30, 30),
                new XYD(13, 30, 40),
                new XYD(14, 30, 50),
                new XYD(15, 30, 10),
                new XYD(16, 30, 20),
                new XYD(17, 30, 30),
                new XYD(18, 30, 40),
                new XYD(19, 30, 50),
                new XYD(20, 30, 10),
                new XYD(21, 30, 20),
                new XYD(22, 30, 30),
                new XYD(23, 30, 40),
                new XYD(24, 30, 50),
                new XYD(25, 30, 10),
                new XYD(26, 30, 20),
                new XYD(27, 30, 30),
                new XYD(28, 30, 40),
                new XYD(29, 30, 50),
                new XYD(30, 30, 10),
                new XYD(31, 30, 20),
                new XYD(32, 30, 30),
                new XYD(33, 30, 40),
                new XYD(34, 30, 50),
                new XYD(35, 30, 10),
                new XYD(36, 30, 20),
                new XYD(37, 30, 30),
                new XYD(38, 30, 40),
                new XYD(39, 30, 50),
                new XYD(40, 30, 10),
                new XYD(41, 30, 20),
                new XYD(42, 30, 30),
                new XYD(43, 30, 40),
                new XYD(44, 30, 50),
                new XYD(45, 30, 10),
                new XYD(46, 30, 20),
                new XYD(47, 30, 30),
                new XYD(48, 30, 40),
                new XYD(49, 30, 50),
                new XYD(50, 30, 10),
                new XYD(51, 30, 20),
                new XYD(52, 30, 30),
                new XYD(53, 30, 40),
                new XYD(54, 30, 50),
                new XYD(55, 30, 10),
                new XYD(56, 30, 20),
                new XYD(57, 30, 30),
                new XYD(58, 30, 40),
                new XYD(59, 30, 50),
                new XYD(60, 30, 10),
                new XYD(61, 30, 20),
                new XYD(62, 30, 30),
                new XYD(63, 30, 40),
                new XYD(64, 30, 50),
                new XYD(65, 30, 10),
                new XYD(66, 30, 20),
                new XYD(67, 30, 30),
                new XYD(68, 30, 40),
                new XYD(69, 30, 50),
                new XYD(70, 30, 10),
                new XYD(71, 30, 20),
                new XYD(72, 30, 30),
                new XYD(73, 30, 40),
                new XYD(74, 30, 50),
                new XYD(75, 30, 10),
                new XYD(76, 30, 20),
                new XYD(77, 30, 30),
                new XYD(78, 30, 40),
                new XYD(79, 30, 50),
                new XYD(80, 30, 10),
                new XYD(81, 30, 20),
                new XYD(82, 30, 30),
                new XYD(83, 30, 40),
                new XYD(84, 30, 50),
                new XYD(85, 30, 10),
                new XYD(86, 30, 20),
                new XYD(87, 30, 30),
                new XYD(88, 30, 40),
                new XYD(89, 30, 50),
                new XYD(90, 30, 10),
                new XYD(91, 30, 20),
                new XYD(92, 30, 30),
                new XYD(93, 30, 40),
                new XYD(94, 30, 50),
                new XYD(95, 30, 10),
                new XYD(96, 30, 20),
                new XYD(97, 30, 30),
                new XYD(98, 30, 40),
                new XYD(99, 30, 50),
            };

            Logger.Write(Logger.D, "---------------------------------------------------------------------------");
            Logger.Write(Logger.D, "真北を上に方向を決める");
            // 公共座標で扱っているので縦軸X、横軸Yで考える（数学軸なら縦軸Y、横軸X）
            double t = Math.Atan2(0 - origin.Y, 1 - origin.X);
            // 指定したモードの範囲内にラジアンを収める
            double d = CoordinateMath.Clamp(2, t);

            Logger.Write(Logger.D, "---------------------------------------------------------------------------");
            Logger.Write(Logger.D, "点群パラメータに最も近い位置に原点を寄せたときの原点座標を求めます");

            double minX = double.NaN;
            double minY = double.NaN;

            double localX = 0.0D;
            double localY = 0.0D;

            int count = pointsL.Count;

            for (int i = 0; i < count; i++) {
                CoordinateMath.ToLocal(pointsL[i].X, pointsL[i].Y, origin.X, origin.Y, -d, ref localX, ref localY);
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

            for (int i = 0; i < count; i++) {
                CoordinateMath.ToLocal(pointsC[i].X, pointsC[i].Y, origin.X, origin.Y, -d, ref localX, ref localY);
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

            for (int i = 0; i < count; i++) {
                CoordinateMath.ToLocal(pointsR[i].X, pointsR[i].Y, origin.X, origin.Y, -d, ref localX, ref localY);
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

            CoordinateMath.ToPublic(minX, minY, origin.X, origin.Y, -d, ref originX, ref originY);

            XYD shiftOrigin = new XYD(originX, originY);
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

            if (meshLength >= maxX / meshPitch - shiftOrigin.X / meshPitch & meshLength >= maxY / meshPitch - shiftOrigin.Y / meshPitch)
                Logger.Write(Logger.D, "indide");
            else
                Logger.Write(Logger.D, $"outside x:{maxX} y:{maxY}");

            DepthArray depthArray = new DepthArray(meshLength, pointsL, pointsC, pointsR, shiftOrigin, meshPitch);
            depthArray.Create();

            sw.Stop();
            Logger.Write(Logger.V, "END");

            Logger.Write(Logger.V, "■処理にかかった時間");
            TimeSpan ts = sw.Elapsed;
            Logger.Write(Logger.V, $"　{ts}");
            Logger.Write(Logger.V, $"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
            Logger.Write(Logger.V, $"　{sw.ElapsedMilliseconds}ミリ秒");

            //int length = depthArray.Depths.Length;
            //double seekStartX = 0.25 + shiftOrigin.X;
            //double seekStartY = 0.25 + shiftOrigin.Y;

            //for (int i = 0; i < length; i++) {
            //    int y = Math.DivRem(i, meshLength, out int x);
            //    Logger.Write(Logger.V, $"X:{ seekStartX + x * meshPitch } Y:{seekStartY + y * meshPitch} Depth:{depthArray.Depths[i] / 10.0}");
            //}

            Console.ReadLine();
        }
    }
}
