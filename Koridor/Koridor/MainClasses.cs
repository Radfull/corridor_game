using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koridor
{
    internal class Chess
    {
        public int posX { get; set; }
        public int posY { get; set; }
        public bool red { get; set; }

        public Chess(int x, int y, bool color)
        {
            posX = x;
            posY = y;
            red = color;
        }

        public Chess() { }
    }
}
