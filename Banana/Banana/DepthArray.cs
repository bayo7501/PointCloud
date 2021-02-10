using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana {
    public class DepthArray {

        private List<XYD> pointsL;
        private List<XYD> pointsC;
        private List<XYD> pointsR;
        private readonly int meshLength;
        private XYD shiftOrigin;
        private readonly double meshPitch;
        private List<Rectangle> rectangles;

        private short[] depths;

        public short[] Depths {
            get => depths;
        }

        public DepthArray(int meshLength, List<XYD> pointsL, List<XYD> pointsC, List<XYD> pointsR, XYD shiftOrigin, double meshPitch) {
            this.meshLength = meshLength;
            this.pointsL = pointsL;
            this.pointsC = pointsC;
            this.pointsR = pointsR;
            this.shiftOrigin = shiftOrigin;
            this.meshPitch = meshPitch;
            int totalLength = meshLength * meshLength;
            depths = new short[totalLength];
        }

        public void Create(){

            Logger.Write(Logger.D, "---------------------------------------------------------------------------");
            Logger.Write(Logger.D, "点群パラメータから小さい四角形を作ります");

            rectangles = new List<Rectangle>();

            // L列の点とC列の点で構成する四角形を作る
            for (int i = 0; i < pointsL.Count; i++) {
                if (i == pointsL.Count - 1)
                    break;
                Rectangle rectangle = new Rectangle(pointsL[i], pointsL[i + 1], pointsC[i + 1], pointsC[i]);
                rectangles.Add(rectangle);
            }

            Logger.Write(Logger.D, $"四角形の数:{rectangles.Count}");

            // C列の点とR列の点で構成する四角形を作る
            for (int i = 0; i < pointsC.Count; i++) {
                if (i == pointsC.Count - 1)
                    break;
                Rectangle rectangle = new Rectangle(pointsC[i], pointsC[i + 1], pointsR[i + 1], pointsR[i]);
                rectangles.Add(rectangle);
            }

            Logger.Write(Logger.D, $"四角形の数:{rectangles.Count}");

            Logger.Write(Logger.D, "---------------------------------------------------------------------------");
            Logger.Write(Logger.D, "点群パラメータの外側の点でが構成するにメッシュの中心座標が含まれているか判定します");
            Logger.Write(Logger.D, "含まれていたら、深さの按分計算を行います");

            // 深さの配列を初期化
            for (int i = 0; i < depths.Length; i++) {
                // defaultは0なので、-1で初期化
                depths[i] = -1;
            }

            // メッシュの中心座標を求め、その中心座標と点群パラメータの外側の点でが構成する矩形との内外判定します

            /*
             *    点群パラメータの外側の点でが構成する矩形
             *    L      C      R
             *     →              
             *    +------+------+ ↓
             *    |      |      |
             *    +------+------+
             *    |      |      |
             *    +------+------+
             *    |      |      |
             * ↑ +------+------+
             *                ← 
             * 外側の点を取得します
             * 
             * 
             */

            // X座標
            List<double> outlineX = new List<double>();
            // X左
            for (int i = 0; i < pointsL.Count; i++)
                // 先頭から順にX座標をリストに追加
                outlineX.Add(pointsL[i].X);
            // X中央末尾
            outlineX.Add(pointsC[pointsC.Count - 1].X);
            // X右
            for (int i = pointsR.Count; i > 0; i--)
                // 末尾から順にX座標をリストに追加
                outlineX.Add(pointsR[i - 1].X);
            // X中央先頭
            outlineX.Add(pointsC[0].X);

            // Y座標
            List<double> outlineY = new List<double>();
            // Y左
            for (int i = 0; i < pointsL.Count; i++)
                // 先頭から順にY座標をリストに追加
                outlineY.Add(pointsL[i].Y);
            // Y中央末尾
            outlineY.Add(pointsC[pointsC.Count - 1].Y);
            // Y右
            for (int i = pointsR.Count; i > 0; i--)
                // 末尾から順にY座標をリストに追加
                outlineY.Add(pointsR[i - 1].Y);
            // Y中央先頭
            outlineY.Add(pointsC[0].Y);

            // 深さ算出処理開始点X
            double seekStartX = 0.25 + shiftOrigin.X;
            // 深さ算出処理開始点Y
            double seekStartY = 0.25 + shiftOrigin.Y;

            for (int y = 0; y < meshLength; y++) {
                for (int x = 0; x < meshLength; x++) {

                    // 走査対象のメッシュ中心点X
                    double seekX = seekStartX + meshPitch * x;
                    // 走査対象のメッシュ中心点Y
                    double seekY = seekStartY + meshPitch * y;
                    Logger.Write(Logger.D, $"走査対象 X:{seekX} Y:{seekY}");

                    int result = CrossCount(seekX, seekY, outlineX, outlineY);
                    if (result == 0) {
                        // 点群パラメータの外側の点で構成する矩形の外側にメッシュの中心がある場合
                        depths[y * meshLength + x] = -1;
                        continue;
                    }

                    // 点群パラメータの外側の点で構成する矩形の内側にメッシュの中心がある場合

                    double depth = 0;

                    for (int i = 0; i < rectangles.Count; i++) {

                        Rectangle rectangle = rectangles[i];

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

                        // さらに小さい四角の単位で現在走査している点が含まれているかチェック
                        if (CrossCount(seekX, seekY, rectangleX, rectangleY) != 0) {

                            // 小さい単位の四角の内側にメッシュの中心がある場合

                            XYD A = new XYD(rectangle.A.X, rectangle.A.Y);
                            XYD C = new XYD(rectangle.C.X, rectangle.C.Y);
                            XYD P = new XYD(seekX, seekY);
                            // 小さい四角のどこにメッシュの中心があるか
                            double ans = CoordinateMath.OnSeg(A, C, P);

                            Logger.Write(Logger.D, $"線分AC A(X:{A.X} Y:{A.Y}) C(X:{C.X} Y:{C.Y})");
                            Logger.Write(Logger.D, $"点P    X:{seekX} Y:{seekY} {ans}");

                            if (ans == 0) {
                                // 線分ACの上

                                XYD _B = rectangle.B;
                                // 線分ACの距離
                                double ac = getDistance(A.X, A.Y, C.X, C.Y);
                                // 点Aから点Pの距離
                                double ap = getDistance(A.X, A.Y, P.X, P.Y);
                                // 点Pから点Cの距離
                                double cp = getDistance(C.X, C.Y, P.X, P.Y);
                                // 点Aと点Cの深さの差
                                double span = Math.Abs(A.D - C.D);
                                if (span == 0) {
                                    Logger.Write(Logger.D, $"{A.D}");
                                    depths[y * meshLength + x] = (short)(A.D * 10.0);
                                    break;
                                } else {
                                    // 点A-点C間の点Pの深さ
                                    double d = span * ap / ac;
                                    // 基準に合わせて加減算して深さを算出
                                    if (A.D > C.D) {
                                        depth = A.D - d;
                                    } else {
                                        depth = A.D + d;
                                    }
                                    Logger.Write(Logger.D, $"{depth}");
                                    depths[y * meshLength + x] = (short)(depth * 10.0);
                                    break;
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
                                if (Intersection(A, C, _B, P, out XYD O)) {
                                    // 線分ACの距離
                                    double ac = getDistance(A.X, A.Y, C.X, C.Y);
                                    // 点Aから点Pの距離
                                    double ao = getDistance(A.X, A.Y, O.X, O.Y);
                                    // 点Pから点Cの距離
                                    double co = getDistance(C.X, C.Y, O.X, O.Y);
                                    // 点Aと点Cの深さの差
                                    double span = Math.Abs(A.D - C.D);
                                    if (span == 0) {
                                        Logger.Write(Logger.D, $"{A.D}");
                                        depths[y * meshLength + x] = (short)(A.D * 10.0);
                                        break;
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
                                        double bo = getDistance(O.X, O.Y, _B.X, _B.Y);
                                        // 点Oから点Pの距離
                                        double po = getDistance(O.X, O.Y, P.X, P.Y);
                                        // 点Bから点Pの距離
                                        double pb = getDistance(_B.X, _B.Y, P.X, P.Y);
                                        // 点Bと点Oの深さの差
                                        span = Math.Abs(O.D - _B.D);
                                        if (span == 0) {
                                            Logger.Write(Logger.D, $"{span}");
                                            depths[y * meshLength + x] = (short)(O.D * 10.0);
                                            break;
                                        } else {
                                            d = span * po / bo;
                                            if (O.D > _B.D) {
                                                depth = O.D - d;
                                            } else {
                                                depth = O.D + d;
                                            }
                                            Logger.Write(Logger.D, $"{depth}");
                                            depths[y * meshLength + x] = (short)(depth * 10.0);
                                            break;
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

                                if (Intersection(A, C, _D, P, out XYD O)) {
                                    double ac = getDistance(A.X, A.Y, C.X, C.Y);
                                    double ao = getDistance(A.X, A.Y, O.X, O.Y);
                                    double co = getDistance(C.X, C.Y, O.X, O.Y);
                                    double span = Math.Abs(A.D - C.D);
                                    if (span == 0) {
                                        Logger.Write(Logger.D, $"{A.D}");
                                        depths[y * meshLength + x] = (short)(A.D * 10.0);
                                        break;
                                    } else {
                                        double d = span * ao / ac;
                                        if (A.D > C.D) {
                                            O.D = A.D - d;
                                        } else {
                                            O.D = A.D + d;
                                        }
                                        double @do = getDistance(O.X, O.Y, _D.X, _D.Y);
                                        double po = getDistance(O.X, O.Y, P.X, P.Y);
                                        double pd = getDistance(_D.X, _D.Y, P.X, P.Y);
                                        span = Math.Abs(O.D - _D.D);
                                        if (span == 0) {
                                            Logger.Write(Logger.D, $"{span}");
                                            depths[y * meshLength + x] = (short)(O.D * 10.0);
                                            break;
                                        } else {
                                            d = span * po / @do;
                                            if (O.D > _D.D) {
                                                depth = O.D - d;
                                            } else {
                                                depth = O.D + d;
                                            }
                                            Logger.Write(Logger.D, $"{depth}");
                                            depths[y * meshLength + x] = (short)(depth * 10.0);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected double getDistance(double x, double y, double x2, double y2) {
            double distance = Math.Sqrt((x2 - x) * (x2 - x) + (y2 - y) * (y2 - y));

            return distance;
        }
        /// <summary>
        /// 内外判定(X軸上はエリア外となる)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="xList"></param>
        /// <param name="yList"></param>
        /// <returns></returns>
        private int CrossCount(double x, double y, List<double> xList, List<double> yList) {
            int crossCount = 0;
            if (xList.Count == yList.Count && xList.Count > 0 && yList.Count > 0) {
                int xCount = xList.Count;

                // 判定位置取得
                double x0value = xList[0];
                double y0value = yList[0];

                bool x0flag = (x <= x0value);
                bool y0flag = (y <= y0value);

                // 判定点
                double x1value = double.NaN;
                double y1value = double.NaN;
                bool x1flag = false;
                bool y1flag = false;

                for (int i = 1; i <= xCount; i++) {
                    // 判定点を取り出し
                    if (i == xCount) {
                        x1value = xList[0];
                        y1value = yList[0];
                    } else {
                        x1value = xList[i];
                        y1value = yList[i];
                    }
                    x1flag = (x <= x1value);
                    y1flag = (y <= y1value);

                    if (y0flag != y1flag) {
                        // 線分はレイを横切る可能性あり。
                        if (x0flag == x1flag) {
                            // 線分の２端点は対象点に対して両方右か両方左にある
                            if (x0flag) {
                                // 完全に右。⇒線分はレイを横切る
                                crossCount += (y0flag ? -1 : 1);
                                // 上から下にレイを横切るときには、交差回数を１引く、下から上は１足す。
                            }
                        } else {
                            // レイと交差するかどうか、対象点と同じ深さで、対象点の右で交差するか、左で交差するかを求める。
                            if (x <= (x0value + (x1value - x0value) * (y - y0value) / (y1value - y0value))) {
                                // 線分は、対象点と同じ深さで、対象点の右で交差する。⇒線分はレイを横切る
                                crossCount += (y0flag ? -1 : 1);
                                // 上から下にレイを横切るときには、交差回数を１引く、下から上は１足す。
                            }
                        }
                    }
                    x0value = x1value;
                    y0value = y1value;
                    x0flag = x1flag;
                    y0flag = y1flag;
                }
            }
            return crossCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0">対角線の点P0</param>
        /// <param name="p1">対角線の点P1</param>
        /// <param name="p2">メッシュの中心の点P2</param>
        /// <param name="p3">対角線と向かい合う点P3</param>
        /// <param name="p">P0-P1とP2-P3の交点</param>
        /// <returns></returns>
        private bool Intersection(XYD p0, XYD p1, XYD p2, XYD p3, out XYD p) {
            p = null;
            double s, t;

            /*
             *      +---+---+---+
             *      |         ／|
             *   -  +       ／  +
             *      |  *  ／    |
             *      +   ／   *  +  +
             *      | ／        |
             *      +---+---+---+
             */

            //s = (p0.X - p1.X) * (p2.Y - p0.Y) + (p0.Y - p1.Y) * (p0.X - p2.X);
            //t = (p0.X - p1.X) * (p3.Y - p0.Y) + (p0.Y - p1.Y) * (p0.X - p3.X);
            //if (s * t >= 0)
            //    return false;

            s = (p2.X - p3.X) * (p0.Y - p2.Y) + (p2.Y - p3.Y) * (p2.X - p0.X);
            t = (p2.X - p3.X) * (p1.Y - p2.Y) + (p2.Y - p3.Y) * (p2.X - p1.X);
            if (s * t >= 0)
                return false;

            // 線分が重なっている場合、交差していない
            // 線分の先端が触れている場合、交差していない
            // 線分が交わって交差した

            // ついでに交点も求める
            double a1 = p0.X - p1.X;
            double b1 = p1.Y - p0.Y;
            double c1 = (p1.Y - p0.Y) * p0.X - (p1.X - p0.X) * p0.Y;

            double a2 = p2.X - p3.X;
            double b2 = p3.Y - p2.Y;
            double c2 = (p3.Y - p2.Y) * p2.X - (p3.X - p2.X) * p2.Y;

            //double x = (c1 * b2 - c2 * b1) / (a1 * b2 - a2 * b1);
            //double y = (a1 * c2 - a2 * c1) / (a1 * b2 - a2 * b1);
            double y = (c1 * b2 - c2 * b1) / (a1 * b2 - a2 * b1);
            double x = (a1 * c2 - a2 * c1) / (a1 * b2 - a2 * b1);

            Logger.Write(Logger.D, $"交点: x={x}, y={y}");

            p = new XYD(x, y);

            return true;
        }

    }
}
