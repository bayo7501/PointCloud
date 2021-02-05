using System;
using System.Collections.Generic;

namespace Apple {
    class Program {
        static void Main(string[] args) {
            XY origin = new XY(0, 0);

            List<XY> pointsL = new List<XY> {
                new XY(10, 10),
                new XY(10, 20),
                new XY(10, 30),
                new XY(10, 40),
                new XY(10, 50)
            };
            List<XY> pointsC = new List<XY> {
                new XY(20, 10),
                new XY(20, 20),
                new XY(20, 30),
                new XY(20, 40),
                new XY(20, 50)
            };
            List<XY> pointsR = new List<XY> {
                new XY(30, 10),
                new XY(30, 20),
                new XY(30, 30),
                new XY(30, 40),
                new XY(30, 50)
            };

            //FunctionalModule module = new FunctionalModule(origin, pointsL, pointsC, pointsR, 5000, 0.5);
            FunctionalModule module = new FunctionalModule(origin, pointsL, pointsC, pointsR, 100, 0.5);
            module.Call();

            Console.ReadLine();
        }
    }
}
