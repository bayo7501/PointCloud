using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace FingerLime {
    class Program {

        /// <summary>
        /// メッシュの長さ(単位:m)
        /// </summary>
        private static readonly int meshLength = 5000;
        /// <summary>
        /// 1目盛りのメッシュの大きさ(単位:m)
        /// </summary>
        private static readonly double meshPitch = 0.5;

        /// <summary>
        /// エントリポイント
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) {

            Logger logger = new Logger(Logger.V);

            var sw = new Stopwatch();
            sw.Start();
            Logger.Write(Logger.V, "■計画レイヤ 作成 START");

            XYD origin = new XYD(0, 0);

            // 1メッシュのサイズは0.5m×0.5m
            // メッシュの数は5000個×5000個
            // つまり長さは2500m×2500m

            // 座標は公共座標、測量軸で扱います。
            // 原点(0,0)に対し、工区の原点に最も近い点P(1,1)を
            // 施工エリア作成開始点として、ここから施工エリアを作成。

            // 施工エリア作成開始点
            int x = 1;
            int y = 1;
            // 深さは一律で10m
            int d = 10;
            /*
             * 作りたいのはこんなカンジ
             *   |-----|-----|-----|
             *   ||---|||---|||---||
             *   ||   |||   |||   ||
             *   ||   |||   |||   ||
             *   ||   |||   |||   ||
             *   ||---|||---|||---||
             *   ||   |||   |||   ||
             *   ||   |||   |||   ||
             *   ||   |||   |||   ||
             *   ||---|||---|||---||
             *   ||   |||   |||   ||
             *   ||   |||   |||   ||
             *   ||   |||   |||   ||
             *   ||---|||---|||---||
             *   ||   |||   |||   ||
             *   ||   |||   |||   ||
             *   ||   |||   |||   ||
             *   ||---|||---|||---||
             *   |-----|-----|-----|
             *   実際にはありえないが、幅2m奥行10mピッチで測量されたと想定。
             */
            List<List<XYD>> xyPoints = new List<List<XYD>>();
            for (int yPitch = 0; yPitch < 1250; yPitch++) {
                // Px 1m → 3m … → 2497m → 2500m

                List<XYD> xPoints = new List<XYD>();

                for (int xPitch = 0; xPitch < 250; xPitch++) {
                    // Px 1m → 11m … → 2489m → 2500m
                    XYD xyd = new XYD(x, y, d);
                    xPoints.Add(xyd);
                    x += 10;
                }
                // X軸の末尾まで到達したら、次にY軸がズレるので開始点に戻しておく
                x = 1;
                xyPoints.Add(xPoints);
                y += 2;
            }

            // 真北を上に
            // 公共座標で扱っているので縦軸X、横軸Yで考える（数学軸なら縦軸Y、横軸X）
            double t = Math.Atan2(0 - origin.Y, 1 - origin.X);
            // 指定したモードの範囲内にラジアンを収める
            double direction = CoordinateMath.Clamp(2, t);

            // 範囲を絞るために計画工区に寄せた原点を求めます。
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

            // 範囲を絞るために計画工区に寄せた原点
            XYD shiftOrigin = new XYD(originX, originY);

            // 5000マス×5000マスのメッシュに収まるかチェックします。
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

            // MeshLength * MeshLength に収まっているか
            // 最も遠い公共座標(単位:m)をマス目換算して 50 / 0.5 = 100
            // シフト原点もマス目換算したもので引き算。
            // 使用するマス目がメッシュサイズの上限を超えていないかチェック

            //if (meshLength >= maxX / meshPitch - shiftOrigin.X / meshPitch & meshLength >= maxY / meshPitch - shiftOrigin.Y / meshPitch)
            //    Logger.Write(Logger.D, "indide");
            //else
            //    Logger.Write(Logger.D, $"outside x:{maxX} y:{maxY}");

            PlanDepthArray planDepth = new PlanDepthArray(meshLength, meshPitch, shiftOrigin, xyPoints);
            planDepth.Create();

            sw.Stop();

            Logger.Write(Logger.V, "■計画レイヤ 作成 処理にかかった時間");
            Logger.Write(Logger.V, $"　{sw.Elapsed}");
            Logger.Write(Logger.V, $"　{sw.Elapsed.Hours}時間 {sw.Elapsed.Minutes}分 {sw.Elapsed.Seconds}秒 {sw.Elapsed.Milliseconds}ミリ秒");
            Logger.Write(Logger.V, $"　{sw.ElapsedMilliseconds}ミリ秒");
            Logger.Write(Logger.V, "■計画レイヤ 作成 END");




            double seekStartX = 0;
            double seekStartY = 0;

            sw.Restart();
            Logger.Write(Logger.V, "■計画レイヤ ファイル出力 START");

            int planLength = planDepth.PlanDepths.Length;
            seekStartX = 0.25 + shiftOrigin.X;
            seekStartY = 0.25 + shiftOrigin.Y;

            if (File.Exists("plan.txt")) {
                File.Delete("plan.txt");
            }
            StreamWriter planWriter = new StreamWriter("plan.txt", true, Encoding.UTF8);
            for (int i = 0; i < planLength; i++) {
                int _y = Math.DivRem(i, meshLength, out int _x);
                try {
                    if (planDepth.PlanDepths[i] == -1) {
                        planWriter.WriteLine($"X:{ seekStartX + _x * meshPitch } Y:{seekStartY + _y * meshPitch} Depth:{planDepth.PlanDepths[i]}");
                    } else {
                        planWriter.WriteLine($"X:{ seekStartX + _x * meshPitch } Y:{seekStartY + _y * meshPitch} Depth:{planDepth.PlanDepths[i] / 10.0}");
                    }

                } catch (Exception e) {
                    Logger.Write(Logger.E, e.ToString());
                    break;
                }
            }
            planWriter.Close();

            sw.Stop();

            Logger.Write(Logger.V, "■計画レイヤ ファイル出力 処理にかかった時間");
            //TimeSpan ts1 = sw.Elapsed;
            Logger.Write(Logger.V, $"　{sw.Elapsed}");
            Logger.Write(Logger.V, $"　{sw.Elapsed.Hours}時間 {sw.Elapsed.Minutes}分 {sw.Elapsed.Seconds}秒 {sw.Elapsed.Milliseconds}ミリ秒");
            Logger.Write(Logger.V, $"　{sw.ElapsedMilliseconds}ミリ秒");
            Logger.Write(Logger.V, "■計画レイヤ ファイル出力 END");



            sw.Restart();
            Logger.Write(Logger.V, "■実績レイヤ 作成 START");

            //XYD prevL = new XYD(5.12, 5.12);
            //XYD prevR = new XYD(8.12, 8.12);
            //XYD currentL = new XYD(8.12, 2.12);
            //XYD currentR = new XYD(11.12, 5.12);

            List<XYD> historyL = new List<XYD>();
            historyL.Add(new XYD(5.12, 5.12, 5));
            historyL.Add(new XYD(8.12, 8.12, 5));
            List<XYD> historyR = new List<XYD>();
            historyR.Add(new XYD(8.12, 2.12, 5));
            historyR.Add(new XYD(11.12, 5.12, 5));

            ActionDepthArray actionDepth = new ActionDepthArray(meshLength, meshPitch, shiftOrigin, planDepth.PlanDepths, historyL, historyR);
            actionDepth.Create();



            sw.Stop();

            Logger.Write(Logger.V, "■実績レイヤ 作成 処理にかかった時間");
            //TimeSpan ts2 = sw.Elapsed;
            Logger.Write(Logger.V, $"　{sw.Elapsed}");
            Logger.Write(Logger.V, $"　{sw.Elapsed.Hours}時間 {sw.Elapsed.Minutes}分 {sw.Elapsed.Seconds}秒 {sw.Elapsed.Milliseconds}ミリ秒");
            Logger.Write(Logger.V, $"　{sw.ElapsedMilliseconds}ミリ秒");
            Logger.Write(Logger.V, "END");



            sw.Restart();
            Logger.Write(Logger.V, "■実績レイヤ ファイル出力 START");

            int length = actionDepth.ActionDepths.Length;
            seekStartX = 0.25 + shiftOrigin.X;
            seekStartY = 0.25 + shiftOrigin.Y;

            if (File.Exists("action.txt")) {
                File.Delete("action.txt");
            }
            StreamWriter actionWriter = new StreamWriter("action.txt", true, Encoding.UTF8);
            for (int i = 0; i < length; i++) {
                int _y = Math.DivRem(i, meshLength, out int _x);
                try {
                    if (actionDepth.ActionDepths[i] == -1) {
                        actionWriter.WriteLine($"X:{ seekStartX + _x * meshPitch } Y:{seekStartY + _y * meshPitch} Depth:{actionDepth.ActionDepths[i]}");
                    } else {
                        actionWriter.WriteLine($"X:{ seekStartX + _x * meshPitch } Y:{seekStartY + _y * meshPitch} Depth:{actionDepth.ActionDepths[i] / 10.0}");
                    }

                } catch (Exception e) {
                    Logger.Write(Logger.E, e.ToString());
                    break;
                }
            }
            actionWriter.Close();

            sw.Stop();

            Logger.Write(Logger.V, "■実績レイヤ ファイル出力 処理にかかった時間");
            //TimeSpan ts1 = sw.Elapsed;
            Logger.Write(Logger.V, $"　{sw.Elapsed}");
            Logger.Write(Logger.V, $"　{sw.Elapsed.Hours}時間 {sw.Elapsed.Minutes}分 {sw.Elapsed.Seconds}秒 {sw.Elapsed.Milliseconds}ミリ秒");
            Logger.Write(Logger.V, $"　{sw.ElapsedMilliseconds}ミリ秒");
            Logger.Write(Logger.V, "■実績レイヤ ファイル出力 END");


            Console.ReadLine();
        }
    }
}
