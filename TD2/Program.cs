using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TD2
{
    class Program
    {
        static void Main()
        {
            string fileName = "./Test001.bmp";
            MyImage image = new MyImage(fileName);
            Console.WriteLine(image.toString());
            Console.WriteLine(image.AffichageMatricePixel());
            image.From_Image_To_File("./Sortie.bmp");
            Console.ReadKey(true);
        }
    }
}
