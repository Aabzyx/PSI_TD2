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
            MyImage image = new MyImage(fileName); //on crée l'objet image avec la classe MyImage avec une image contenu dans le debug
            Console.WriteLine(image.toString()); //on affiche les informations de l'image
            Console.WriteLine(image.AffichageMatricePixel()); //on affiche la matrice de pixel
            image.From_Image_To_File("./Sortie.bmp"); //on crée la nouvelle image à partir de l'image initiale
            Console.ReadKey(true);
        }
    }
}
