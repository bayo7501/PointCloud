using System;
using System.Collections.Generic;
using System.Linq;

namespace Apple {

    public class FunctionalModule {

        /// <summary>
        /// 点群L
        /// </summary>
        private readonly List<XY> pointsL = null;
        /// <summary>
        /// 点群C
        /// </summary>
        private readonly List<XY> pointsC = null;
        /// <summary>
        /// 点群R
        /// </summary>
        private readonly List<XY> pointsR = null;

        /// <summary>
        /// 原点
        /// </summary>
        private readonly XY origin = null;

        /// <summary>
        /// メッシュの長さ
        /// </summary>
        private readonly int meshLength = 5000;
        /// <summary>
        /// 1目盛りのメッシュの大きさ
        /// </summary>
        private readonly double meshPitch = 0.5;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="pointsL"></param>
        /// <param name="pointsC"></param>
        /// <param name="pointsR"></param>
        /// <param name="meshLength"></param>
        /// <param name="meshPitch"
        public FunctionalModule(XY origin, List<XY> pointsL, List<XY> pointsC, List<XY> pointsR, int meshLength, double meshPitch) {
            this.origin = origin;
            this.pointsL = pointsL;
            this.pointsC = pointsC;
            this.pointsR = pointsR;
            this.meshLength = meshLength;
            this.meshPitch = meshPitch;
        }

        public void Call() {

            Console.WriteLine(string.Empty);
            Console.WriteLine("---------------------------------------------------------------------------");

            double d = 0.0;
            double t = Math.Atan2(0 - origin.Y, 1 - origin.X); // 公共座標で扱っているので縦軸X、横軸Yで考える（数学軸なら縦軸Y、横軸X）
            Console.WriteLine(string.Format("atan2 = {0}", t));
            Console.WriteLine(string.Format("ラジアンから度へ:{0}度", ToAngle(t)));
            // 指定したモードの範囲内にラジアンを収める
            d = Clamp(2, t);
            Console.WriteLine(string.Format("正規化した値:{0}", d));
            Console.WriteLine(string.Format("ラジアンから度へ:{0}度", ToAngle(d)));

            Console.WriteLine(string.Empty);
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine("原点を点群パラメータに寄せたときの原点座標を求めます");

            if (pointsL == null)
                Console.WriteLine("Lリストがnull");
            if (pointsC == null)
                Console.WriteLine("Cリストがnull");
            if (pointsR == null)
                Console.WriteLine("Rリストがnull");

            if (pointsL.Count == 0)
                Console.WriteLine("Lリストの長さが0");
            if (pointsC.Count == 0)
                Console.WriteLine("Cリストの長さが0");
            if (pointsR.Count == 0)
                Console.WriteLine("Rリストの長さが0");

            double minX = double.NaN;
            double minY = double.NaN;

            double localX = 0.0D;
            double localY = 0.0D;

            bool b = false;

            for (int i = 0; i < 3; i++) {
                if (i == 0)
                    b = ToLocal(pointsL[i].X, pointsL[i].Y, origin.X, origin.Y, -d, ref localX, ref localY);
                else if (i == 1)
                    b = ToLocal(pointsC[i].X, pointsC[i].Y, origin.X, origin.Y, -d, ref localX, ref localY);
                else if (i == 2)
                    b = ToLocal(pointsR[i].X, pointsR[i].Y, origin.X, origin.Y, -d, ref localX, ref localY);

                if (!b)
                    return;

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
            b = ToPublic(minX, minY, origin.X, origin.Y, -d, ref originX, ref originY);
            if (!b)
                return;
            XY shiftOrigin = new XY(originX, originY);

            Console.WriteLine(string.Format("原点座標 X:0 → {0}, Y:0 → {1}", shiftOrigin.X, shiftOrigin.Y));

            Console.WriteLine(string.Empty);
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine("点群パラメータが5000×5000のメッシュに収まるかチェックします");

            double maxX = pointsL.Max(a => a.X);
            if (maxX < pointsC.Max(a => a.X))
                maxX = pointsC.Max(a => a.X);
            if (maxX < pointsL.Max(a => a.X))
                maxX = pointsL.Max(a => a.X);

            double maxY = pointsL.Max(a => a.Y);
            if (maxX < pointsC.Max(a => a.Y))
                maxX = pointsC.Max(a => a.Y);
            if (maxX < pointsL.Max(a => a.Y))
                maxX = pointsL.Max(a => a.Y);

            Console.WriteLine(string.Format("点群パラメータのうち最も遠い点 X:{0} Y:{1}", maxX, maxY));
            // 5000 * 5000 に収まっているか
            if ((meshPitch + shiftOrigin.X) * meshLength < meshPitch * maxX)
                Console.WriteLine(string.Format("X範囲外:{0}", maxX));
            else
                Console.WriteLine("X 範囲内");

            if ((meshPitch + shiftOrigin.Y) * meshLength < meshPitch * maxY)
                Console.WriteLine(string.Format("Y範囲外:{0}", maxY));
            else
                Console.WriteLine("Y 範囲内");

            Console.WriteLine(string.Empty);
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine("点群パラメータから小さい四角形を作ります");

            List<Rectangle> rectangles = new List<Rectangle>();
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

            // 要素の0-2または1-3を結べば対角線を引くことができる

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
             *                      ←
             * ↓ +---+---+---+---+---+
             *    |   |   |   |   |   |
             *    +---+---+---+---+---+
             *    |   |   |   |   |   |
             *    +---+---+---+---+---+
             *    |   |   |   |   |   |
             *    +---+---+---+---+---+ ↑
             *     →            
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
                    int result = PointInArea(seekX, seekY, outlineX, outlineY);
                    if (result == 0) {
                        // 外
                        //Console.WriteLine(string.Format("X:{0} Y:{1} {2}", seekX, seekY, "outside"));
                    } else {

                        // 内
                        //Console.WriteLine(string.Format("X:{0} Y:{1} {2}", seekX, seekY, "inside"));
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
                            if (PointInArea(seekX, seekY, rectangleX, rectangleY) != 0) {
                                // 四角の内
                                Pt p0 = new Pt(rectangle.P0.X, rectangle.P0.Y);
                                Pt p2 = new Pt(rectangle.P2.X, rectangle.P2.Y);
                                Pt p = new Pt(seekX, seekY);
                                double j = Pt.OnSeg2(p0, p2, p);

                                Console.WriteLine(string.Format("線分AB A(X:{0} Y:{1}) B(X:{2} Y:{3})", p0.X, p0.Y, p2.X, p2.Y));
                                Console.WriteLine(string.Format("点P    X:{0} Y:{1} {2}", seekX, seekY, j));

                                if (j == 0) {
                                    Console.WriteLine("オンザライン");
                                } else if (j > 0) {
                                    Console.WriteLine("プラス");

                                    XY A = rectangle.P0;
                                    XY B = rectangle.P2;
                                    XY O = rectangle.P1;
                                    XY xy = new XY(seekX, seekY);

                                    XY pt;
                                    if (Intersection(A, B, O, xy, out pt)) {
                                    }
                                } else if (j < 0) {
                                    Console.WriteLine("マイナス");
                                    XY A = rectangle.P0;
                                    XY B = rectangle.P2;
                                    XY O = rectangle.P3;
                                    XY xy = new XY(seekX, seekY);

                                    XY pt;
                                    if (Intersection(A, B, O, xy, out pt)) {
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

            //Console.WriteLine(string.Empty);
            //Console.WriteLine("---------------------------------------------------------------------------");
            //Console.WriteLine("点群パラメータに含まれていたメッシュの中心座標と三角形の底辺から深さを案分計算します");
        }

        /// <summary>
        /// 度からラジアンへ
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        static double ToRadian(double angle) {
            return (double)(angle * Math.PI / 180);
        }

        /// <summary>
        /// ラジアンから度へ
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        static double ToAngle(double radian) {
            return (double)(radian * 180 / Math.PI);
        }

        /// <summary>
        /// ラジアンの正規化
        /// </summary>
        /// <param name="mode">
        /// 1:0～360
        /// 2:-180～180
        /// </param>
        /// <param name="radian">ラジアン</param>
        /// <returns>変換後のラジアン</returns>
        static double Clamp(int mode, double radian) {
            double value = radian;
            double l = Math.PI * 2.0D;

            if (mode == 1) {
                while (value < 0.0)
                    value += l;
                while (value >= l)
                    value -= l;
            } else if (mode == 2) {
                while (value < -Math.PI)
                    value += l;
                while (value >= Math.PI)
                    value -= l;
            }
            return value;
        }

        /// <summary>
        /// 測量公共XY座標から測量ローカル座標に変換
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="shiftX"></param>
        /// <param name="shiftY"></param>
        /// <param name="radian"></param>
        /// <param name="outX"></param>
        /// <param name="outY"></param>
        /// <returns></returns>
        static bool ToLocal(double x, double y, double shiftX, double shiftY, double radian, ref double outX, ref double outY) {
            try {
                // シフト
                double localShiftX = x - shiftX;
                double localShiftY = y - shiftY;
                localShiftX = ToRoundDown(localShiftX + 0.00001, 4);
                localShiftY = ToRoundDown(localShiftY + 0.00001, 4);

                // 回転
                double localX = Math.Cos(radian) * localShiftX - Math.Sin(radian) * localShiftY;
                double localY = Math.Sin(radian) * localShiftX + Math.Cos(radian) * localShiftY;
                localX = ToRoundDown(localX + 0.00001, 4);
                localY = ToRoundDown(localY + 0.00001, 4);

                outX = localX;
                outY = localY;
                return true;
            } catch (Exception e) {
                Console.WriteLine(string.Format("{0}", e.ToString()));
                return false;
            }
        }

        /// <summary>
        /// 測量ローカル座標から測量公共XY座標に変換(画面回転考慮)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="shiftX"></param>
        /// <param name="shiftY"></param>
        /// <param name="radian"></param>
        /// <param name="outX"></param>
        /// <param name="outY"></param>
        /// <returns></returns>
        static bool ToPublic(double x, double y, double shiftX, double shiftY, double radian, ref double outX, ref double outY) {
            try {
                // 回転
                double localX = Math.Cos(-radian) * x - Math.Sin(-radian) * y;
                double localY = Math.Sin(-radian) * x + Math.Cos(-radian) * y;
                localX = ToRoundDown(localX + 0.00001, 4);
                localY = ToRoundDown(localY + 0.00001, 4);

                // シフト
                double localShiftX = localX + shiftX;
                double localShiftY = localY + shiftY;
                localShiftX = ToRoundDown(localShiftX + 0.00001, 4);
                localShiftY = ToRoundDown(localShiftY + 0.00001, 4);

                outX = localShiftX;
                outY = localShiftY;
                return true;
            } catch (Exception e) {
                Console.WriteLine(string.Format("{0}", e.ToString()));
                return false;
            }
        }

        /// <summary>
        /// 指定した精度の数値に切り捨て
        /// </summary>
        /// <param name="dValue">丸め対象の倍精度浮動小数点数</param>
        /// <param name="iDigits">戻り値の有効桁数の精度</param>
        /// <returns>入力された精度の数値に切り捨てられた数値</returns>
        static double ToRoundDown(double dValue, int iDigits) {
            double dCoef = Math.Pow(10, iDigits);
            return dValue > 0 ? Math.Floor(dValue * dCoef) / dCoef : Math.Ceiling(dValue * dCoef) / dCoef;
        }

        private static bool Intersection(XY p0, XY p1, XY p2, XY p3, out XY p) {
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

            p = new XY(x, y);

            return true;
        }

        //static bool Intersection(POINT p1, POINT p2, POINT p3, POINT p4) {

        //    double s, t;
        //    s = (p1.x - p2.x) * (p3.y - p1.y) + (p1.y - p2.y) * (p1.x - p3.x);
        //    t = (p1.x - p2.x) * (p4.y - p1.y) + (p1.y - p2.y) * (p1.x - p4.x);
        //    if (s * t >= 0)
        //        return false;

        //    s = (p3.x - p4.x) * (p1.y - p3.y) + (p3.y - p4.y) * (p3.x - p1.x);
        //    t = (p3.x - p4.x) * (p2.y - p3.y) + (p3.y - p4.y) * (p3.x - p2.x);
        //    if (s * t >= 0)
        //        return false;

        //    // 線分が重なっている場合、交差していない
        //    // 線分の先端が触れている場合、交差していない
        //    // 線分が交わって交差した

        //    // ついでに交点も求める
        //    double a1 = p1.x - p2.x;
        //    double b1 = p2.y - p1.y;
        //    double c1 = (p2.y - p1.y) * p1.x - (p2.x - p1.x) * p1.y;

        //    double a2 = p3.x - p4.x;
        //    double b2 = p4.y - p3.y;
        //    double c2 = (p4.y - p3.y) * p3.x - (p4.x - p3.x) * p3.y;

        //    double x = (c1 * b2 - c2 * b1) / (a1 * b2 - a2 * b1);
        //    double y = (a1 * c2 - a2 * c1) / (a1 * b2 - a2 * b1);

        //    Console.WriteLine(string.Format("交点: x={0}, y={1}", x, y));

        //    return true;
        //}

        //struct POINT {
        //    public double x;
        //    public double y;
        //}

        /// <summary>
        /// 内外判定(X軸上はエリア外となる)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="xList"></param>
        /// <param name="yList"></param>
        /// <returns></returns>
        static int PointInArea(double x, double y, List<double> xList, List<double> yList) {
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

        public List<XY> PointsL {
            get => pointsL;
        }

        public List<XY> PointsC {
            get => pointsC;
        }

        public List<XY> PointsR {
            get => pointsR;
        }

        public XY Origin {
            get => origin;
        }

        public double MeshLength {
            get => meshLength;
        }

        public double MeshPitch {
            get => meshPitch;
        }
    }
}
