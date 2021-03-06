﻿using System;

namespace DragonFruit {
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
                Logger.Write(Logger.W, $"{e.ToString()}");
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
                Logger.Write(Logger.W, $"{e.ToString()}");
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
    }
}
