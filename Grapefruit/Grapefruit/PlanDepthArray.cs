using System;
using System.Collections.Generic;

namespace Grapefruit {

    /// <summary>
    /// 計画メッシュレイヤ
    /// </summary>
    public class PlanDepthArray {

        /// <summary>
        /// 5000マス×5000マス
        /// </summary>
        private readonly int meshLength;

        /// <summary>
        /// 0.5m/1マス
        /// </summary>
        private readonly double meshPitch;

        /// <summary>
        /// 範囲を絞るために計画工区に寄せた原点
        /// </summary>
        private Pnt shiftOrigin;

        /// <summary>
        /// 
        /// </summary>
        private List<Pnt> planPnts;
        /// <summary>
        /// 
        /// </summary>
        private List<Face> planFaces;

        /// <summary>
        /// 計画 メッシュ中心深さ
        /// </summary>
        private short[] planDepths;

        /// <summary>
        /// 計画 メッシュ中心深さ(get only)
        /// </summary>
        public short[] PlanDepths {
            get => planDepths;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="meshLength">5000マス×5000マス</param>
        /// <param name="meshPitch">0.5m/1マス</param>
        /// <param name="shiftOrigin">範囲を絞るために計画工区に寄せた原点</param>
        /// <param name="planPnts"></param>
        /// <param name="planFaces"></param>
        public PlanDepthArray(int meshLength, double meshPitch, Pnt shiftOrigin, List<Pnt> planPnts, List<Face> planFaces) {
            this.meshLength = meshLength;
            this.meshPitch = meshPitch;
            this.shiftOrigin = shiftOrigin;
            this.planPnts = planPnts;
            this.planFaces = planFaces;

            int totalLength = meshLength * meshLength;
            planDepths = new short[totalLength];
        }

        /// <summary>
        /// 計画 メッシュ中心の深さデータを作成します
        /// </summary>
        public void Create() {

            // 深さの配列を初期化
            for (int i = 0; i < planDepths.Length; i++) {
                // defaultは0なので、-1で初期化
                planDepths[i] = -1;
            }

            double seekY = 0;
            double seekX = 0;

            try {
                for (int i = 0; i < planFaces.Count; i++) {
                    Face face = planFaces[i];

                    // Faceの3点から外接する四角形の4点をとる
                    double minX = face.A.X;
                    if (minX > face.B.X) {
                        minX = face.B.X;
                    }
                    if (minX > face.C.X) {
                        minX = face.C.X;
                    }

                    double minifyMinX = Math.Floor(minX);

                    double minY = face.A.Y;
                    if (minY > face.B.Y) {
                        minY = face.B.Y;
                    }
                    if (minY > face.C.Y) {
                        minY = face.C.Y;
                    }

                    double minifyMinY = Math.Floor(minY);

                    double maxX = face.A.X;
                    if (maxX < face.B.X) {
                        maxX = face.B.X;
                    }
                    if (maxX < face.C.X) {
                        maxX = face.C.X;
                    }

                    double minifyMaxX = Math.Ceiling(maxX);

                    double maxY = face.A.Y;
                    if (maxY < face.B.Y) {
                        maxY = face.B.Y;
                    }
                    if (maxY < face.C.Y) {
                        maxY = face.C.Y;
                    }
                    double minifyMaxY = Math.Ceiling(maxY);
                    //// 外接する四角形の4点
                    //XYD minifyA = new XYD(minX, minY);
                    //XYD minifyB = new XYD(maxX, minY);
                    //XYD minifyC = new XYD(maxX, maxY);
                    //XYD minifyD = new XYD(minX, maxY);

                    int meshLengthY = (int)((minifyMaxY - minifyMinY) / 0.5);
                    int meshLengthX = (int)((minifyMaxX - minifyMinX) / 0.5);
                    // 深さ算出処理開始点X
                    double seekStartX = 0.25 + minifyMinX;
                    // 深さ算出処理開始点Y
                    double seekStartY = 0.25 + minifyMinY;

                    for (int y = 0; y < meshLengthY; y++) {
                        // 走査対象のメッシュ中心点Y
                        seekY = seekStartY + meshPitch * y;

                        for (int x = 0; x < meshLengthX; x++) {
                            double depth = 0;

                            // 走査対象のメッシュ中心点X
                            seekX = seekStartX + meshPitch * x;

                            List<double> triangleX = new List<double>();
                            triangleX.Add(face.A.X);
                            triangleX.Add(face.B.X);
                            triangleX.Add(face.C.X);

                            List<double> triangleY = new List<double>();
                            triangleY.Add(face.A.Y);
                            triangleY.Add(face.B.Y);
                            triangleY.Add(face.C.Y);

                            // 小さい四角の単位で現在走査している点が含まれているかチェック
                            if (CoordinateMath.CrossCount(seekX, seekY, triangleX, triangleY) == 0)
                                // 含まれていなかった
                                continue;

                            // 含まれていた
                            Pnt p = new Pnt(seekX, seekY);
                            double ans = CoordinateMath.OnSeg(face.A, face.C, p);
                            if (ans == 0) {
                                // 線分ACの上

                                // 線分ACの距離
                                double ac = CoordinateMath.Distance(face.A.X, face.A.Y, face.C.X, face.C.Y);
                                // 点Aから点Pの距離
                                double ap = CoordinateMath.Distance(face.A.X, face.A.Y, p.X, p.Y);
                                // 点Pから点Cの距離
                                double cp = CoordinateMath.Distance(face.C.X, face.C.Y, p.X, p.Y);
                                // 点Aと点Cの深さの差
                                double span = Math.Abs(face.A.D - face.C.D);
                                if (span == 0) {
                                    planDepths[(int)(Math.Floor((seekY - shiftOrigin.Y) / meshPitch) * meshLength + Math.Floor((seekX - shiftOrigin.X) / meshPitch))] = (short)(face.A.D * 100.0);
                                    continue;
                                } else {
                                    // 点A-点C間の点Pの深さ
                                    double d = span * ap / ac;
                                    // 基準に合わせて加減算して深さを算出
                                    if (face.A.D > face.C.D) {
                                        depth = face.A.D - d;
                                    } else {
                                        depth = face.A.D + d;
                                    }
                                    planDepths[(int)(Math.Floor((seekY - shiftOrigin.Y) / meshPitch) * meshLength + Math.Floor((seekX - shiftOrigin.X) / meshPitch))] = (short)(depth * 100.0);
                                    continue;
                                }
                            } else {

                                // 点Bとメッシュの中心点Pを通った線分AC上の交点Oを求める
                                if (CoordinateMath.Intersection(face.A, face.C, face.B, p, out Pnt O)) {
                                    // 線分ACの距離
                                    double ac = CoordinateMath.Distance(face.A.X, face.A.Y, face.C.X, face.C.Y);
                                    // 点Aから点Pの距離
                                    double ao = CoordinateMath.Distance(face.A.X, face.A.Y, O.X, O.Y);
                                    // 点Pから点Cの距離
                                    double co = CoordinateMath.Distance(face.C.X, face.C.Y, O.X, O.Y);
                                    // 点Aと点Cの深さの差
                                    double span = Math.Abs(face.A.D - face.C.D);
                                    if (span == 0) {
                                        planDepths[(int)(Math.Floor((seekY - shiftOrigin.Y) / meshPitch) * meshLength + Math.Floor((seekX - shiftOrigin.X) / meshPitch))] = (short)(face.A.D * 100.0);
                                        continue;
                                    } else {
                                        // 点A-点C間の点Pの深さ
                                        double d = span * ao / ac;
                                        // 基準に合わせて加減算して深さを算出
                                        if (face.A.D > face.C.D) {
                                            O.D = face.A.D - d;
                                        } else {
                                            O.D = face.A.D + d;
                                        }

                                        // 線分BOの距離
                                        double bo = CoordinateMath.Distance(O.X, O.Y, face.B.X, face.B.Y);
                                        // 点Oから点Pの距離
                                        double po = CoordinateMath.Distance(O.X, O.Y, p.X, p.Y);
                                        // 点Bから点Pの距離
                                        double pb = CoordinateMath.Distance(face.B.X, face.B.Y, p.X, p.Y);
                                        // 点Bと点Oの深さの差
                                        span = Math.Abs(O.D - face.B.D);
                                        if (span == 0) {
                                            planDepths[(int)(Math.Floor((seekY - shiftOrigin.Y) / meshPitch) * meshLength + Math.Floor((seekX - shiftOrigin.X) / meshPitch))] = (short)(O.D * 100.0);
                                            continue;
                                        } else {
                                            d = span * po / bo;
                                            if (O.D > face.B.D) {
                                                depth = O.D - d;
                                            } else {
                                                depth = O.D + d;
                                            }
                                            planDepths[(int)(Math.Floor((seekY - shiftOrigin.Y) / meshPitch) * meshLength + Math.Floor((seekX - shiftOrigin.X) / meshPitch))] = (short)(depth * 100.0);
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            } catch (Exception e) {

                Console.WriteLine($"{seekX},{seekY},{e.ToString()}");
            }

        }
    }
}
