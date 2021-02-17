using System;
using System.Collections.Generic;

namespace FingerLime {
    class CoordinateMath {

        /// <summary>
        /// 度からラジアンへ
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double ToRadian(double angle) {
            return (double)(angle * Math.PI / 180);
        }

        /// <summary>
        /// ラジアンから度へ
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static double ToAngle(double radian) {
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
        public static double Clamp(int mode, double radian) {
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
        public static bool ToLocal(double x, double y, double shiftX, double shiftY, double radian, ref double outX, ref double outY) {
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
                //Logger.Write(Logger.W, $"{e.ToString()}");
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
        public static bool ToPublic(double x, double y, double shiftX, double shiftY, double radian, ref double outX, ref double outY) {
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
                //Logger.Write(Logger.W, $"{e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 指定した精度の数値に切り捨て
        /// </summary>
        /// <param name="dValue">丸め対象の倍精度浮動小数点数</param>
        /// <param name="iDigits">戻り値の有効桁数の精度</param>
        /// <returns>入力された精度の数値に切り捨てられた数値</returns>
        public static double ToRoundDown(double dValue, int iDigits) {
            double dCoef = Math.Pow(10, iDigits);
            return dValue > 0 ? Math.Floor(dValue * dCoef) / dCoef : Math.Ceiling(dValue * dCoef) / dCoef;
        }

        /// <summary>
        /// 平面での線と点の位置関係
        /// ・｜
        /// ｜・
        /// 
        /// 点p1 - 点p2 を結ぶ線分の左右どちらに 点q があるか?
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static double OnSeg(XYD p1, XYD p2, XYD q) {
            double vx1 = p2.X - p1.X;
            double vy1 = p2.Y - p1.Y;
            double vx2 = q.X - p1.X;
            double vy2 = q.Y - p1.Y;
            double ans = vx1 * vy2 - vy1 * vx2;
            return ans;
        }

        /// <summary>
        /// 内外判定(X軸上はエリア外となる)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="xList"></param>
        /// <param name="yList"></param>
        /// <returns></returns>
        public static int CrossCount(double x, double y, List<double> xList, List<double> yList) {
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
        /// 距離計算
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static double Distance(double x, double y, double x2, double y2) {
            double distance = Math.Sqrt((x2 - x) * (x2 - x) + (y2 - y) * (y2 - y));

            return distance;
        }

        /// <summary>
        /// 四角形の対角線とある線分の延長上で交わる場合の交点を求めます。
        /// </summary>
        /// <param name="p0">対角線の点P0</param>
        /// <param name="p1">対角線の点P1</param>
        /// <param name="p2">メッシュの中心の点P2</param>
        /// <param name="p3">対角線と向かい合う点P3</param>
        /// <param name="p">P0-P1とP2-P3の交点</param>
        /// <returns>四角形の対角線とある線分の延長上で交わる場合、真</returns>
        public static bool Intersection(XYD p0, XYD p1, XYD p2, XYD p3, out XYD p) {
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

            p = new XYD(x, y);

            return true;
        }

    }
}
