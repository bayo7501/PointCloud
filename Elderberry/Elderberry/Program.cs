﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Elderberry {
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


            int x = 1;
            int y = 1;
            int d = 10;
            List<List<XYD>> xyPoints = new List<List<XYD>>();
            for (int yPitch = 0; yPitch < 1250; yPitch++) {
                List<XYD> xPoints = new List<XYD>();
                for (int xPitch = 0; xPitch < 250; xPitch++) {
                    XYD xyd = new XYD(x, y, d);
                    xPoints.Add(xyd);
                    x += 20;
                }
                x = 1;
                xyPoints.Add(xPoints);
                y += 4;
            }

            Logger.Write(Logger.D, "---------------------------------------------------------------------------");
            Logger.Write(Logger.D, "真北を上に方向を決める");
            // 公共座標で扱っているので縦軸X、横軸Yで考える（数学軸なら縦軸Y、横軸X）
            double t = Math.Atan2(0 - origin.Y, 1 - origin.X);
            // 指定したモードの範囲内にラジアンを収める
            double direction = CoordinateMath.Clamp(2, t);

            Logger.Write(Logger.D, "---------------------------------------------------------------------------");
            Logger.Write(Logger.D, "点群パラメータに最も近い位置に原点を寄せたときの原点座標を求めます");

            double minX = double.NaN;
            double minY = double.NaN;

            double localX = 0.0D;
            double localY = 0.0D;

            int xyCount = xyPoints.Count;
            for (int i = 0; i < xyCount; i++) {

                List<XYD> xPoints = xyPoints[i];

                int xCount = xPoints.Count;
                for (int j = 0; j < xCount; j++) {

                    CoordinateMath.ToLocal(xPoints[j].X, xPoints[j].Y, origin.X, origin.Y, -direction, ref localX, ref localY);
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
            }

            double originX = 0.0;
            double originY = 0.0;

            CoordinateMath.ToPublic(minX, minY, origin.X, origin.Y, -direction, ref originX, ref originY);

            XYD shiftOrigin = new XYD(originX, originY);
            Logger.Write(Logger.D, $"原点座標 X:0 → {shiftOrigin.X}, Y:0 → {shiftOrigin.Y}");

            Logger.Write(Logger.D, "---------------------------------------------------------------------------");
            Logger.Write(Logger.D, $"点群パラメータが {meshLength}×{meshLength} のメッシュに収まるかチェックします");
            Logger.Write(Logger.D, $"メートル換算で {meshLength * meshPitch}m × {meshLength * meshPitch}m");

            double maxX = double.NaN;
            double maxY = double.NaN;
            for (int i = 0; i < xyPoints.Count; i++) {
                List<XYD> xPoints = xyPoints[i];
                double seekMaxX = xPoints.Max(a => a.X);

                double seekMaxY = xPoints.Max(a => a.Y);

                if (seekMaxX > maxX || double.IsNaN(maxX))
                    maxX = seekMaxX;

                if (seekMaxY > maxY || double.IsNaN(maxY))
                    maxY = seekMaxY;

            }

            Logger.Write(Logger.D, $"点群パラメータのうち最も遠い点 X:{maxX} Y:{maxY}");

            // MeshLength * MeshLength に収まっているか
            // 最も遠い公共座標(単位:m)をマス目換算して 50 / 0.5 = 100
            // シフト原点もマス目換算したもので引き算。
            // 使用するマス目がメッシュサイズの上限を超えていないかチェック

            if (meshLength >= maxX / meshPitch - shiftOrigin.X / meshPitch & meshLength >= maxY / meshPitch - shiftOrigin.Y / meshPitch)
                Logger.Write(Logger.D, "indide");
            else
                Logger.Write(Logger.D, $"outside x:{maxX} y:{maxY}");

            DepthArray depthArray = new DepthArray(meshLength, xyPoints, shiftOrigin, meshPitch);
            depthArray.Create();

            sw.Stop();
            Logger.Write(Logger.V, "END");

            Logger.Write(Logger.V, "■処理にかかった時間");
            TimeSpan ts = sw.Elapsed;
            Logger.Write(Logger.V, $"　{ts}");
            Logger.Write(Logger.V, $"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
            Logger.Write(Logger.V, $"　{sw.ElapsedMilliseconds}ミリ秒");

            int length = depthArray.Depths.Length;
            double seekStartX = 0.25 + shiftOrigin.X;
            double seekStartY = 0.25 + shiftOrigin.Y;

            //for (int i = 0; i < length; i++) {
            //    int _y = Math.DivRem(i, meshLength, out int _x);
            //    Logger.Write(Logger.V, $"X:{ seekStartX + _x * meshPitch } Y:{seekStartY + _y * meshPitch} Depth:{depthArray.Depths[i] / 10.0}");
            //}

            Console.ReadLine();
        }
    }
}
