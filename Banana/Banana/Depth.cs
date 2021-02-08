using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana {
    public class Depth {

        private List<Pt> pointsL;
        private List<Pt> pointsC;
        private List<Pt> pointsR;
        private int meshLength;
        private Pt shiftOrigin;
        private double meshPitch;
        private List<Rectangle> rectangles;

        public Depth(int meshLength, List<Pt> pointsL, List<Pt> pointsC, List<Pt> pointsR, Pt shiftOrigin, double meshPitch) {
            this.meshLength = meshLength;
            this.pointsL = pointsL;
            this.pointsC = pointsC;
            this.pointsR = pointsR;
            this.shiftOrigin = shiftOrigin;
            this.meshPitch = meshPitch;
        }

        public void Init() {
            Console.WriteLine(string.Empty);
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine("点群パラメータから小さい四角形を作ります");

            rectangles = new List<Rectangle>();

            // L列の点とC列の点で構成する四角形を作る
            for (int i = 0; i < pointsL.Count; i++) {
                if (i == pointsL.Count - 1)
                    break;
                Rectangle rectangle = new Rectangle(pointsL[i], pointsL[i + 1], pointsC[i + 1], pointsC[i]);
                rectangles.Add(rectangle);
            }
            Console.WriteLine(string.Format("四角形の数:{0}", rectangles.Count));
            // C列の点とR列の点で構成する四角形を作る
            for (int i = 0; i < pointsC.Count; i++) {
                if (i == pointsC.Count - 1)
                    break;
                Rectangle rectangle = new Rectangle(pointsC[i], pointsC[i + 1], pointsR[i + 1], pointsR[i]);
                rectangles.Add(rectangle);
            }
            Console.WriteLine(string.Format("四角形の数:{0}", rectangles.Count));
        }

        public void Execute(){
            Console.WriteLine(string.Empty);
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine("点群パラメータの外側の点でが構成するにメッシュの中心座標が含まれているか判定します");
            Console.WriteLine("含まれていたら、深さの按分計算を行います");

            // 深さの配列を初期化
            int totalLength = meshLength * meshLength;
            short[] depths = new short[totalLength];
            for (int i = 0; i < depths.Length; i++) {
                // defaultは0なので、-1で初期化
                depths[i] = -1;
            }
            //Console.ReadLine();

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
            for (int i = pointsR.Count; i < 0; i--)
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
            for (int i = pointsR.Count; i < 0; i--)
                // 末尾から順にY座標をリストに追加
                outlineY.Add(pointsR[i - 1].Y);
            // Y中央先頭
            outlineY.Add(pointsC[0].Y);

            double seekStartX = 0.25 + shiftOrigin.X;
            double seekStartY = 0.25 + shiftOrigin.Y;

            for (int x = 0; x < meshLength; x++) {
                for (int y = 0; y < meshLength; y++) {
                    double seekX = seekStartX + x * meshPitch;
                    double seekY = seekStartY + y * meshPitch;
                    int result = CrossCount(seekX, seekY, outlineX, outlineY);
                    if (result == 0) {
                        // 点群パラメータの外側の点で構成する矩形の外側にメッシュの中心がある場合
                        //Console.WriteLine(string.Format("X:{0} Y:{1} {2}", seekX, seekY, "outside"));
                    } else {

                        // 点群パラメータの外側の点で構成する矩形の内側にメッシュの中心がある場合
                        //Console.WriteLine(string.Format("X:{0} Y:{1} {2}", seekX, seekY, "inside"));

                        // さらに小さい単位でどの四角に含まれているか
                        for (int i = 0; i < rectangles.Count; i++) {

                            Rectangle rectangle = rectangles[i];
                            List<double> rectangleX = new List<double>();
                            rectangleX.Add(rectangle.P0.X);
                            rectangleX.Add(rectangle.P1.X);
                            rectangleX.Add(rectangle.P2.X);
                            rectangleX.Add(rectangle.P3.X);
                            List<double> rectangleY = new List<double>();
                            rectangleY.Add(rectangle.P0.Y);
                            rectangleY.Add(rectangle.P1.Y);
                            rectangleY.Add(rectangle.P2.Y);
                            rectangleY.Add(rectangle.P3.Y);
                            if (CrossCount(seekX, seekY, rectangleX, rectangleY) != 0) {
                                // 小さい単位の四角の内側にメッシュの中心がある場合
                                Pt p0 = new Pt(rectangle.P0.X, rectangle.P0.Y);
                                Pt p2 = new Pt(rectangle.P2.X, rectangle.P2.Y);
                                Pt p = new Pt(seekX, seekY);
                                double j = Pt.OnSeg2(p0, p2, p);

                                Console.WriteLine(string.Format("線分AB A(X:{0} Y:{1}) B(X:{2} Y:{3})", p0.X, p0.Y, p2.X, p2.Y));
                                Console.WriteLine(string.Format("点P    X:{0} Y:{1} {2}", seekX, seekY, j));

                                // 小さい単位の資格のどこにメッシュの中心があるか
                                if (j == 0) {
                                    // 線上
                                    Console.WriteLine("オンザライン");
                                } else if (j > 0) {
                                    // プラス側
                                    Console.WriteLine("プラス");

                                    Pt A = rectangle.P0;
                                    Pt B = rectangle.P2;
                                    Pt O = rectangle.P1;
                                    Pt xy = new Pt(seekX, seekY);

                                    Pt pt;
                                    // 対角線に向かい合う点とメッシュの中心を通った対角線の交点座標
                                    if (Intersection(A, B, O, xy, out pt)) {
                                        // 対角線の両端の深さ
                                        double ab = getDistance(A.X, A.Y, B.X, B.Y);
                                        // 対角線の両端と対角線上の交点までの長さと割合
                                        double ap = getDistance(A.X, A.Y, pt.X, pt.Y);
                                        double bp = getDistance(B.X, B.Y, pt.X, pt.Y);

                                        double span = Math.Abs(A.D - B.D);
                                        if (span == 0) {
                                            Console.WriteLine(string.Format("{0}", A.D));
                                        } else {
                                            // 深さを算出
                                            double w = span * ap / ab;
                                            double depth = 0;
                                            if (A.D > B.D) {
                                                depth = A.D - w;
                                            } else {
                                                depth = A.D + w;
                                            }
                                            Console.WriteLine(string.Format("{0}", depth));
                                        }
                                    }
                                } else if (j < 0) {
                                    // マイナス側
                                    Console.WriteLine("マイナス");
                                    Pt A = rectangle.P0;
                                    Pt B = rectangle.P2;
                                    Pt O = rectangle.P3;
                                    Pt xy = new Pt(seekX, seekY);

                                    Pt pt;
                                    if (Intersection(A, B, O, xy, out pt)) {
                                        // 対角線の両端の深さ
                                        double ab = getDistance(A.X, A.Y, B.X, B.Y);
                                        // 対角線の両端と対角線上の交点までの長さと割合
                                        double ap = getDistance(A.X, A.Y, pt.X, pt.Y);
                                        double bp = getDistance(B.X, B.Y, pt.X, pt.Y);

                                        double span = Math.Abs(A.D - B.D);
                                        if (span == 0) {
                                            Console.WriteLine(string.Format("{0}", A.D));
                                        } else {
                                            // 深さを算出
                                            double w = span * ap / ab;
                                            double depth = 0;
                                            if (A.D > B.D) {
                                                depth = A.D - w;
                                            } else {
                                                depth = A.D + w;
                                            }
                                            Console.WriteLine(string.Format("{0}", depth));
                                        }
                                    }
                                }
                                break;
                            }

                        }

                    }
                }
            }


            Console.WriteLine(string.Empty);
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine("点群パラメータに含まれていたメッシュの中心座標と三角形の底辺から深さを案分計算します");
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
                            // レイと交差するかどうか、対象点と同じ高さで、対象点の右で交差するか、左で交差するかを求める。
                            if (x <= (x0value + (x1value - x0value) * (y - y0value) / (y1value - y0value))) {
                                // 線分は、対象点と同じ高さで、対象点の右で交差する。⇒線分はレイを横切る
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

        private bool Intersection(Pt p0, Pt p1, Pt p2, Pt p3, out Pt p) {
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

            double x = (c1 * b2 - c2 * b1) / (a1 * b2 - a2 * b1);
            double y = (a1 * c2 - a2 * c1) / (a1 * b2 - a2 * b1);

            Console.WriteLine(string.Format("交点: x={0}, y={1}", x, y));

            p = new Pt(x, y);

            return true;
        }

    }
}
