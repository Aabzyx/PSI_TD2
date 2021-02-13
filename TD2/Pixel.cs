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
        private int red; //octet correspondant à la couleur rouge
        private int green; //octet correspondant à la couleur verte
        private int blue; //octet correspondant à la couleur bleue
        public Pixel(int red, int green, int blue)
        {
            this.blue = blue;
            this.green = green;
            this.red = red;
        }
        public int Red //propriété retournant l'octet rouge
        {
            get { return red; }
        }
        public int Green //propriété retournant l'octet vert
        {
            get { return green; }
        }
        public int Blue //propriété retournant l'octet bleue
        {
            get { return blue; }
        }
        public string toString() //méthode toString classique
        {
            return red + " " + green + " " + blue + " ";
        }
    }
}
