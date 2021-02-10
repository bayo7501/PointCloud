using System;

namespace Banana {

    class Logger {

        /// <summary>
        /// none
        /// </summary>
        public static uint N = 0x00000000;
        /// <summary>
        /// erro
        /// </summary>
        public static uint E = 0x10000000;
        /// <summary>
        /// warn
        /// </summary>
        public static uint W = 0x01000000;
        /// <summary>
        /// info
        /// </summary>
        public static uint I = 0x00001000;
        /// <summary>
        /// debug
        /// </summary>
        public static uint D = 0x00000010;
        /// <summary>
        /// verbose
        /// </summary>
        public static uint V = 0x00000001;

        public static uint _level = E;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="level">出力レベル(複数設定した場合、I + Dで両方出力可)</param>
        public Logger(uint level) {
            _level = level;
        }

        public static void Write(uint level, string message) {
            if ((level & _level) != 0) {
                //System.Diagnostics.Debug.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}");
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}");
            }
        }
    }
}
