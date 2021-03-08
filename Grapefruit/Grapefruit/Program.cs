using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Grapefruit {
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
            Console.WriteLine("■START");

            //XMLReader reader = new XMLReader(@"./akt2新工事.xml");
            XMLReader reader = new XMLReader(@"./5000かけ5000.xml");
            reader.ReadXML("Surfaces");
            List<Face> faces = reader.Faces;
            List<Pnt> pnts = reader.Pnts;

            //for (int i = 0; i < faces.Count; i++) {
            //    Console.WriteLine($"{faces[i].A.ID} {faces[i].B.ID} {faces[i].C.ID}");
            //}

            // 原点
            Pnt origin = new Pnt(0, 0);

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

            int pntCount = pnts.Count;
            for (int i = 0; i < pntCount; i++) {

                Pnt pnt = pnts[i];


                CoordinateMath.ToLocal(pnt.X, pnt.Y, origin.X, origin.Y, -direction, ref localX, ref localY);
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

            CoordinateMath.ToPublic(minX, minY, origin.X, origin.Y, -direction, ref originX, ref originY);

            // 範囲を絞るために計画工区に寄せた原点
            Pnt shiftOrigin = new Pnt(originX, originY);

            // 5000マス×5000マスのメッシュに収まるかチェックします。
            double maxX = double.NaN;
            double maxY = double.NaN;
            for (int i = 0; i < pnts.Count; i++) {
                Pnt pnt = pnts[i];

                if (pnt.X > maxX || double.IsNaN(maxX))
                    maxX = pnt.X;

                if (pnt.Y > maxY || double.IsNaN(maxY))
                    maxY = pnt.Y;
            }

            // MeshLength * MeshLength に収まっているか
            // 最も遠い公共座標(単位:m)をマス目換算して 50 / 0.5 = 100
            // シフト原点もマス目換算したもので引き算。
            // 使用するマス目がメッシュサイズの上限を超えていないかチェック

            if (meshLength >= maxX / meshPitch - shiftOrigin.X / meshPitch & meshLength >= maxY / meshPitch - shiftOrigin.Y / meshPitch)
                Console.WriteLine("inside");
            else
                Console.WriteLine($"outside x:{maxX} y:{maxY}");


            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("■深さ計算処理 開始");
            PlanDepthArray planDepth = new PlanDepthArray(meshLength, meshPitch, shiftOrigin, pnts, faces);
            planDepth.Create();

            sw.Stop();

            Console.WriteLine("■深さ計算処理 にかかった時間");
            Console.WriteLine($"　{sw.Elapsed}");
            Console.WriteLine($"　{sw.Elapsed.Hours}時間 {sw.Elapsed.Minutes}分 {sw.Elapsed.Seconds}秒 {sw.Elapsed.Milliseconds}ミリ秒");
            Console.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");
            Console.WriteLine("■深さ計算処理 終了");

            double seekStartX = 0;
            double seekStartY = 0;

            Console.WriteLine("■ファイル出力 START");

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
                        planWriter.WriteLine($"X:{ seekStartX + _x * meshPitch } Y:{seekStartY + _y * meshPitch} Depth:{planDepth.PlanDepths[i] / 100.0}");
                    }

                } catch (Exception e) {
                    Logger.Write(Logger.E, e.ToString());
                    break;
                }
            }
            planWriter.Close();

            Console.WriteLine("■ファイル出力 END");

            Console.ReadKey();
        }
    }
}
