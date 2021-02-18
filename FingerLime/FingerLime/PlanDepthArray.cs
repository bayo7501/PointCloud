using System;
using System.Collections.Generic;

namespace FingerLime {

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
        private XYD shiftOrigin;

        /// <summary>
        /// 計画工区データ(公共のX軸リスト×Y軸リスト)
        /// </summary>
        private List<List<XYD>> xyPoints;

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
        /// <param name="xyPoints">計画工区データ(公共のX軸リスト×Y軸リスト)</param>
        public PlanDepthArray(int meshLength, double meshPitch, XYD shiftOrigin, List<List<XYD>> xyPoints) {
            this.meshLength = meshLength;
            this.meshPitch = meshPitch;
            this.shiftOrigin = shiftOrigin;
            this.xyPoints = xyPoints;

            int totalLength = meshLength * meshLength;
            planDepths = new short[totalLength];
        }

        /// <summary>
        /// 計画 メッシュ中心の深さデータを作成します
        /// </summary>
        public void Create() {

            double seekY = 0;
            double seekX = 0;

            try {
                // 計画工区から小さく切り出した四角形の工区
                List<Rectangle> rectangles = new List<Rectangle>();

                // 隣あう列の点で構成する四角形を作る
                for (int y = 0; y < xyPoints.Count; y++) {
                    if (y == xyPoints.Count - 1)
                        break;
                    List<XYD> aPoints = xyPoints[y];
                    List<XYD> bPoints = xyPoints[y + 1];

                    for (int x = 0; x < aPoints.Count; x++) {
                        if (x == aPoints.Count - 1)
                            continue;

                        Rectangle rectangle = new Rectangle(aPoints[x], aPoints[x + 1], bPoints[x + 1], bPoints[x]);
                        rectangles.Add(rectangle);
                    }
                }

                // 深さの配列を初期化
                for (int i = 0; i < planDepths.Length; i++) {
                    // defaultは0なので、-1で初期化
                    planDepths[i] = -1;
                }

                for (int i = 0; i < rectangles.Count; i++) {
                    Rectangle rectangle = rectangles[i];

                    // 計画工区から小さく切り出した四角形の工区から
                    // 外接する四角形の4点をとる
                    double minX = rectangle.A.X;
                    if (minX > rectangle.B.X) {
                        minX = rectangle.B.X;
                    }
                    if (minX > rectangle.C.X) {
                        minX = rectangle.C.X;
                    }
                    if (minX > rectangle.D.X) {
                        minX = rectangle.D.X;
                    }

                    double minifyMinX = Math.Floor(minX);

                    double minY = rectangle.A.Y;
                    if (minY > rectangle.B.Y) {
                        minY = rectangle.B.Y;
                    }
                    if (minY > rectangle.C.Y) {
                        minY = rectangle.C.Y;
                    }
                    if (minY > rectangle.D.Y) {
                        minY = rectangle.D.Y;
                    }

                    double minifyMinY = Math.Floor(minY);

                    double maxX = rectangle.A.X;
                    if (maxX < rectangle.B.X) {
                        maxX = rectangle.B.X;
                    }
                    if (maxX < rectangle.C.X) {
                        maxX = rectangle.C.X;
                    }
                    if (maxX < rectangle.D.X) {
                        maxX = rectangle.D.X;
                    }

                    double minifyMaxX = Math.Ceiling(maxX);

                    double maxY = rectangle.A.Y;
                    if (maxY < rectangle.B.Y) {
                        maxY = rectangle.B.Y;
                    }
                    if (maxY < rectangle.C.Y) {
                        maxY = rectangle.C.Y;
                    }
                    if (maxY < rectangle.D.Y) {
                        maxY = rectangle.D.Y;
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

                            List<double> rectangleX = new List<double>();
                            rectangleX.Add(rectangle.A.X);
                            rectangleX.Add(rectangle.B.X);
                            rectangleX.Add(rectangle.C.X);
                            rectangleX.Add(rectangle.D.X);

                            List<double> rectangleY = new List<double>();
                            rectangleY.Add(rectangle.A.Y);
                            rectangleY.Add(rectangle.B.Y);
                            rectangleY.Add(rectangle.C.Y);
                            rectangleY.Add(rectangle.D.Y);

                            // 小さい四角の単位で現在走査している点が含まれているかチェック
                            if (CoordinateMath.CrossCount(seekX, seekY, rectangleX, rectangleY) == 0)
                                // 含まれていなかった
                                continue;

                            // 含まれていた
                            XYD A = new XYD(rectangle.A.X, rectangle.A.Y, rectangle.A.D);
                            XYD C = new XYD(rectangle.C.X, rectangle.C.Y, rectangle.C.D);
                            XYD P = new XYD(seekX, seekY);
                            // 小さい四角のどこにメッシュの中心があるか
                            double ans = CoordinateMath.OnSeg(A, C, P);

                            if (ans == 0) {
                                // 線分ACの上

                                XYD _B = rectangle.B;
                                // 線分ACの距離
                                double ac = CoordinateMath.Distance(A.X, A.Y, C.X, C.Y);
                                // 点Aから点Pの距離
                                double ap = CoordinateMath.Distance(A.X, A.Y, P.X, P.Y);
                                // 点Pから点Cの距離
                                double cp = CoordinateMath.Distance(C.X, C.Y, P.X, P.Y);
                                // 点Aと点Cの深さの差
                                double span = Math.Abs(A.D - C.D);
                                if (span == 0) {
                                    planDepths[(int)(Math.Floor((seekY - shiftOrigin.Y) / meshPitch) * meshLength + Math.Floor((seekX - shiftOrigin.X) / meshPitch))] = (short)(A.D * 10.0);
                                    continue;
                                } else {
                                    // 点A-点C間の点Pの深さ
                                    double d = span * ap / ac;
                                    // 基準に合わせて加減算して深さを算出
                                    if (A.D > C.D) {
                                        depth = A.D - d;
                                    } else {
                                        depth = A.D + d;
                                    }
                                    planDepths[(int)(Math.Floor((seekY - shiftOrigin.Y) / meshPitch) * meshLength + Math.Floor((seekX - shiftOrigin.X) / meshPitch))] = (short)(depth * 10.0);
                                    continue;
                                }

                            } else if (ans < 0) {

                                /*
                                    *    B* +---+---+---+  C*
                                    *       |           |
                                    *       +           +
                                    *       |           |
                                    *       +           +
                                    *       |           |
                                    *    A* +---+---+---+  D
                                    */

                                XYD _B = rectangle.B;

                                // 点Bとメッシュの中心点Pを通った線分AC上の交点Oを求める
                                if (CoordinateMath.Intersection(A, C, _B, P, out XYD O)) {
                                    // 線分ACの距離
                                    double ac = CoordinateMath.Distance(A.X, A.Y, C.X, C.Y);
                                    // 点Aから点Pの距離
                                    double ao = CoordinateMath.Distance(A.X, A.Y, O.X, O.Y);
                                    // 点Pから点Cの距離
                                    double co = CoordinateMath.Distance(C.X, C.Y, O.X, O.Y);
                                    // 点Aと点Cの深さの差
                                    double span = Math.Abs(A.D - C.D);
                                    if (span == 0) {
                                        planDepths[(int)(Math.Floor((seekY - shiftOrigin.Y) / meshPitch) * meshLength + Math.Floor((seekX - shiftOrigin.X) / meshPitch))] = (short)(A.D * 10.0);
                                        continue;
                                    } else {
                                        // 点A-点C間の点Pの深さ
                                        double d = span * ao / ac;
                                        // 基準に合わせて加減算して深さを算出
                                        if (A.D > C.D) {
                                            O.D = A.D - d;
                                        } else {
                                            O.D = A.D + d;
                                        }

                                        // 線分BOの距離
                                        double bo = CoordinateMath.Distance(O.X, O.Y, _B.X, _B.Y);
                                        // 点Oから点Pの距離
                                        double po = CoordinateMath.Distance(O.X, O.Y, P.X, P.Y);
                                        // 点Bから点Pの距離
                                        double pb = CoordinateMath.Distance(_B.X, _B.Y, P.X, P.Y);
                                        // 点Bと点Oの深さの差
                                        span = Math.Abs(O.D - _B.D);
                                        if (span == 0) {
                                            planDepths[(int)(Math.Floor((seekY - shiftOrigin.Y) / meshPitch) * meshLength + Math.Floor((seekX - shiftOrigin.X) / meshPitch))] = (short)(O.D * 10.0);
                                            continue;
                                        } else {
                                            d = span * po / bo;
                                            if (O.D > _B.D) {
                                                depth = O.D - d;
                                            } else {
                                                depth = O.D + d;
                                            }
                                            planDepths[(int)(Math.Floor((seekY - shiftOrigin.Y) / meshPitch) * meshLength + Math.Floor((seekX - shiftOrigin.X) / meshPitch))] = (short)(depth * 10.0);
                                            continue;
                                        }
                                    }
                                }
                            } else if (ans > 0) {

                                /*
                                    *    B  +---+---+---+  C*
                                    *       |           |
                                    *       +           +
                                    *       |           |
                                    *       +           +
                                    *       |           |
                                    *    A* +---+---+---+  D*
                                    */

                                XYD _D = rectangle.D;

                                if (CoordinateMath.Intersection(A, C, _D, P, out XYD O)) {
                                    double ac = CoordinateMath.Distance(A.X, A.Y, C.X, C.Y);
                                    double ao = CoordinateMath.Distance(A.X, A.Y, O.X, O.Y);
                                    double co = CoordinateMath.Distance(C.X, C.Y, O.X, O.Y);
                                    double span = Math.Abs(A.D - C.D);
                                    if (span == 0) {
                                        planDepths[(int)(Math.Floor((seekY - shiftOrigin.Y) / meshPitch) * meshLength + Math.Floor((seekX - shiftOrigin.X) / meshPitch))] = (short)(A.D * 10.0);
                                        continue;
                                    } else {
                                        double d = span * ao / ac;
                                        if (A.D > C.D) {
                                            O.D = A.D - d;
                                        } else {
                                            O.D = A.D + d;
                                        }
                                        double @do = CoordinateMath.Distance(O.X, O.Y, _D.X, _D.Y);
                                        double po = CoordinateMath.Distance(O.X, O.Y, P.X, P.Y);
                                        double pd = CoordinateMath.Distance(_D.X, _D.Y, P.X, P.Y);
                                        span = Math.Abs(O.D - _D.D);
                                        if (span == 0) {
                                            planDepths[(int)(Math.Floor((seekY - shiftOrigin.Y) / meshPitch) * meshLength + Math.Floor((seekX - shiftOrigin.X) / meshPitch))] = (short)(O.D * 10.0);
                                            continue;
                                        } else {
                                            d = span * po / @do;
                                            if (O.D > _D.D) {
                                                depth = O.D - d;
                                            } else {
                                                depth = O.D + d;
                                            }
                                            planDepths[(int)(Math.Floor((seekY - shiftOrigin.Y) / meshPitch) * meshLength + Math.Floor((seekX - shiftOrigin.X) / meshPitch))] = (short)(depth * 10.0);
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } catch (Exception e) {
                //Logger.Write(Logger.E, $"{seekX},{seekY},{e.ToString()}");
            }
        }
    }
}
