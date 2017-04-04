using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XNACS1Lib;

namespace Splendor
{
    class MyLibrary
    {
        public static int nWindowPixelWidth = 1100;
        public static int nWindowPixelHeight = 780;

        public static int nMaxHandSize = 3;
        public int nMaxCoins = 10;

        public static float Resize(int nPixelSize)
        {
            return XNACS1Base.World.WorldDimension.X / nWindowPixelWidth * nPixelSize;
        }
    }
}
