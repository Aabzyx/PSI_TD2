using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TD2
{
    class MyImage
    {
        private string typeImage;
        private int tailleDuFichier;
        private int tailleOffset;
        private int largeurImage;
        private int hauteurImage;
        private int nombreBytesParCouleur;
        private Pixel[,] pixels;

        public MyImage(string myFile)
        {
            byte[] image = File.ReadAllBytes(myFile);
            typeImage = TypeDImage(image);
            tailleDuFichier = Convertir_Endian_To_Int(image, 4, 2);
            tailleOffset = Convertir_Endian_To_Int(image, 4, 14);
            largeurImage = Convertir_Endian_To_Int(image, 4, 18);
            hauteurImage = Convertir_Endian_To_Int(image, 4, 22);
            nombreBytesParCouleur = Convertir_Endian_To_Int(image, 2, 28);
            pixels = new Pixel[hauteurImage, largeurImage];
            Remplir_pixels(pixels, image);
        }

        public void Remplir_pixels(Pixel[,] pixels, byte[] image)
        {
            int indice = 14 + Convert.ToInt32(tailleOffset);
            int valeursInutiles = largeurImage % 4;
            for (int l = 0; l < pixels.GetLength(0); l++)
            {
                for(int c = 0; c < pixels.GetLength(1); c++)
                {
                    Pixel nouveauPixel = new Pixel(image[indice + 2], image[indice + 1], image[indice]);
                    pixels[l, c] = nouveauPixel;
                    indice += 3;
                }
                if (valeursInutiles != 0)
                {
                    if (valeursInutiles == 1)
                    {
                        indice++;
                    }
                    if (valeursInutiles == 2)
                    {
                        indice += 2;
                    }
                    if (valeursInutiles == 3)
                    {
                        indice += 3;
                    }
                }
            }
        }

        public Pixel[,] Pixels
        {
            get { return pixels; }
        }

        public void From_Image_To_File(string file)
        {
            byte[] returned = new byte[tailleDuFichier];
            returned[0] = Convert.ToByte(66);
            returned[1] = Convert.ToByte(77);
            byte[] tailleDuFichierEndian = Convertir_Int_To_Endian(tailleDuFichier, 4);
            for (int i = 2; i < 6; i++)
            {
                returned[i] = tailleDuFichierEndian[i - 2];
            }
            returned[10] = Convert.ToByte(14 + tailleOffset);
            byte[] tailleOffsetEndian = Convertir_Int_To_Endian(tailleOffset, 4);
            for (int i = 14; i < 18; i++)
            {
                returned[i] = tailleOffsetEndian[i - 14];
            }
            byte[] largeurImageEndian = Convertir_Int_To_Endian(largeurImage, 4);
            for (int i = 18; i < 22; i++)
            {
                returned[i] = largeurImageEndian[i - 18];
            }
            byte[] hauteurImageEndian = Convertir_Int_To_Endian(hauteurImage, 4);
            for (int i = 22; i < 26; i++)
            {
                returned[i] = hauteurImageEndian[i - 22];
            }
            returned[26] = Convert.ToByte(1);
            byte[] nombreBytesParCouleurEndian = Convertir_Int_To_Endian(nombreBytesParCouleur, 2);
            for (int i = 28; i < 30; i++)
            {
                returned[i] = nombreBytesParCouleurEndian[i - 28];
            }
            byte[] tailleImageEndian = Convertir_Int_To_Endian(tailleDuFichier - 14 - tailleOffset, 4);
            for (int i = 34; i < 38; i++)
            {
                returned[i] = tailleImageEndian[i - 34];
            }

            int cpt = tailleOffset + 14;
            int valeursInutiles = largeurImage % 4;
            for(int i = 0; i < hauteurImage; i++)
            {
                for(int j = 0; j < largeurImage; j++)
                {
                    returned[cpt] = Convert.ToByte(pixels[i, j].Blue);
                    returned[cpt + 1] = Convert.ToByte(pixels[i, j].Green);
                    returned[cpt + 2] = Convert.ToByte(pixels[i, j].Red);
                    cpt += 3;
                }
                if(valeursInutiles != 0)
                {
                    if(valeursInutiles == 1)
                    {
                        cpt++;
                    }
                    if(valeursInutiles == 2)
                    {
                        cpt += 2;
                    }
                    if(valeursInutiles == 3)
                    {
                        cpt += 3;
                    }
                }
            }

            File.WriteAllBytes(file, returned);
        }

        public string toString()
        {
            string returned = "";
            returned += "Type de l'image : " + typeImage + "\n";
            returned += "Taille de l'image : " + Convert.ToString(tailleDuFichier) + "\n";
            returned += "Taille du header d'info : " + Convert.ToString(tailleOffset) + "\n";
            returned += "Largeur de l'image : " + Convert.ToString(largeurImage) + "\n";
            returned += "Hauteur de l'image : " + Convert.ToString(hauteurImage) + "\n";
            returned += "Nomvre de bytes par couleur : " + Convert.ToString(nombreBytesParCouleur) + "\n";
            return returned;
        }

        public string AffichageMatricePixel()
        {
            string returned = "";
            for(int x = 0; x < pixels.GetLength(0); x++)
            {
                for(int y = 0; y < pixels.GetLength(1); y++)
                {
                    returned += pixels[x,y].toString();
                }
                returned += "\n";
            }
            return returned;
        }

        private string TypeDImage(byte[] image)
        {
            string returned = "";
            if (image[0] == 66 && image[1] == 77)
            {
                returned = "BM";
            }
            else returned = "Inconnu";
            return returned;
        }

        public int Convertir_Endian_To_Int(byte[] image, int nombreOctets, int indiceDepart)
        {
            int returned = 0;
            for(int x = 0; x < nombreOctets; x++)
            {
                returned += Convert.ToInt32(image[indiceDepart + x] * Math.Pow(256, x));
            }
            return returned;
        }

        public byte[] Convertir_Int_To_Endian(int val, int nombreOctets)
        {
            byte[] returned = new byte[nombreOctets];
            for(int x = nombreOctets - 1; x >= 0; x--)
            {
                returned[x] = Convert.ToByte(val / Math.Pow(256, x));
                val = Convert.ToInt32(val % Math.Pow(256, x));
            }
            return returned;
        }
    }
}
