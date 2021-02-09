using System;

namespace Banana {
    class Logger {

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
        public static uint D = 0x00000001;
        /// <summary>
        /// performance
        /// </summary>
        public static uint P = 0x00000100;

        public static uint _level = E;

        public Logger(uint level) {
            _level = level;
        }

        public static void Write(uint level, string message) {
            if ((level & _level) != 0) {

                System.Diagnostics.Debug.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}");
                //Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}");
            }
        }
    }
}
