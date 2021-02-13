using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TD2
{
    class Pixel
    {
        private int red;
        private int green;
        private int blue;
        public Pixel(int red, int green, int blue)
        {
            this.blue = blue;
            this.green = green;
            this.red = red;
        }
        public int Red
        {
            get { return red; }
        }
        public int Green
        {
            get { return green; }
        }
        public int Blue
        {
            get { return blue; }
        }
        public string toString()
        {
            return red + " " + green + " " + blue + " ";
        }
    }
}
