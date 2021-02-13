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
        private string typeImage; //extension du fichier
        private int tailleDuFichier; //taille du fichier en entier en octets
        private int tailleOffset; //taille offset en entier en octets
        private int largeurImage; //largeur de l'image en entier (nombre de pixels horizontal)
        private int hauteurImage; //hauteur de l'image en entier (nombre de pixels vertical)
        private int nombreBytesParCouleur; //permet de voir le nombre d'octet
        private Pixel[,] pixels; //matrice dite de Pixel permet de s'y retrouver plus facilement sur une image, la matrice à la même largeur et hauteur que l'image et chaque case contient les trois composantes Rouge Vert et Bleu accessible via une propriété

        public MyImage(string myFile)
        {
            byte[] image = File.ReadAllBytes(myFile); //lecture du fichier image bytes par bytes et classement dans un tableau de byte
            typeImage = TypeDImage(image); //permet de voir si l'extension du fichier lu juste avant est du type bitmap ou d'un autre autre type
            tailleDuFichier = Convertir_Endian_To_Int(image, 4, 2); //permet de convertir la taille du fichier en little endian dans les meta données en entier, à partir de l'octet 2 sur 4 octets
            tailleOffset = Convertir_Endian_To_Int(image, 4, 14); //convertit la taille de l'offset dans les méta données en entier, à partir de l'octet 14 sur 4 octets
            largeurImage = Convertir_Endian_To_Int(image, 4, 18); //convertit la largeur de l'image dans les méta données en entier, à partir de l'octet 18 sur 4 octets
            hauteurImage = Convertir_Endian_To_Int(image, 4, 22); //convertit l'hauteur de l'image dans les méta données en entier, à partir de l'octet 22 sur 4 octets
            nombreBytesParCouleur = Convertir_Endian_To_Int(image, 2, 28); //convertit le nombre d'octet par pixel dans les méta données en entier, à partir de l'octet 28 sur 2 octets
            pixels = new Pixel[hauteurImage, largeurImage];
            Remplir_pixels(pixels, image);
        }

        public void Remplir_pixels(Pixel[,] pixels, byte[] image) //cette méthode permet de remplir notre matrice de pixel, qui nous permettre de faciliter notre travail par la suite
        {
            int indice = 14 + Convert.ToInt32(tailleOffset); //on initialise notre indice au premier octet de l'image, c'est à dire au rang de l'entête du fichier + celui de l'image (taille de l'offeset)
            int valeursInutiles = largeurImage % 4; //dans le cas ou nous avons une image qui n'est pas multiple de 4 en hauteur*largeur, il faut ignorer soit 1,2 ou 3 octets (afin de passer vers la ligne de pixel suivante)
            for (int l = 0; l < pixels.GetLength(0); l++)
            {
                for(int c = 0; c < pixels.GetLength(1); c++)
                {
                    Pixel nouveauPixel = new Pixel(image[indice + 2], image[indice + 1], image[indice]); //on tri chaque pixel de l'image dans notre matrice de pixel, on fait attention à l'ordre des couleurs dans le fichier BGR contre RGB dans notre matrice
                    pixels[l, c] = nouveauPixel; //on ajoute le nouvel objet pixel dans notre matrice de pixel
                    indice += 3; //on incrémente de 3 pour passer aux trois octets suivants sur l'image correspondant au pixel suivant
                }
                if (valeursInutiles != 0) //le cas ou nous n'avons pas une image multiple de 4 en hauteur*largeur
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

        public Pixel[,] Pixels //propriété permettant de retourner la matrice de pixel
        {
            get { return pixels; }
        }

        public void From_Image_To_File(string file) //méthode permettant de créer une image à partir des méta données de notre image initiale et de notre matrice de pixel
        {
            byte[] returned = new byte[tailleDuFichier]; //on initialise le tableau d'octet correspondant à la nouvelle image
            returned[0] = Convert.ToByte(66); //on entre un "B" en byte
            returned[1] = Convert.ToByte(77); //on entre un "M" en byte 
            byte[] tailleDuFichierEndian = Convertir_Int_To_Endian(tailleDuFichier, 4); //on copie colle la taille du fichier initiale dans le nouveau fichier en le convertissant en endian
            for (int i = 2; i < 6; i++)
            {
                returned[i] = tailleDuFichierEndian[i - 2]; //on rentre la taille du fichier en little endian dans les méta données de la nouvelle image dans les octets 2 et 5
            }
            returned[10] = Convert.ToByte(14 + tailleOffset); //on convertit en byte la taille de l'offset en octet provenant de l'image d'origine
            byte[] tailleOffsetEndian = Convertir_Int_To_Endian(tailleOffset, 4); //puis on converti la taille de l'offset en little endian
            for (int i = 14; i < 18; i++)
            {
                returned[i] = tailleOffsetEndian[i - 14]; //on copie colle taille de l'offset précédemment converti dans les méta données de la nouvelle image entre les octets 14 et 17
            }
            byte[] largeurImageEndian = Convertir_Int_To_Endian(largeurImage, 4); //on convertit la largeur de l'image en little endian
            for (int i = 18; i < 22; i++)
            {
                returned[i] = largeurImageEndian[i - 18]; //on rentre la largeur de l'image dans les octets correspondants dans les méta données de la  nouvelle image (entre 18 et 21)
            }
            byte[] hauteurImageEndian = Convertir_Int_To_Endian(hauteurImage, 4); //on fait de même avec la hauteur de l'image
            for (int i = 22; i < 26; i++)
            {
                returned[i] = hauteurImageEndian[i - 22]; //et on le met dans les octets 22 à 25 de la nouvelle image
            }
            returned[26] = Convert.ToByte(1); //on met le nombre de plan à 1 dans l'octet 26 de la nouvelle image
            byte[] nombreBytesParCouleurEndian = Convertir_Int_To_Endian(nombreBytesParCouleur, 2); //convertit en little endian le nombre d'octet par couleur de l'image original
            for (int i = 28; i < 30; i++)
            {
                returned[i] = nombreBytesParCouleurEndian[i - 28]; //puis on l'insére dans les octets correspondant dans la nouvelle image. (octet 28 à 29)
            }
            byte[] tailleImageEndian = Convertir_Int_To_Endian(tailleDuFichier - 14 - tailleOffset, 4); //on convertit la taille de l'image en little endian (en prenant soin d'enlever la taille de l'offset et des données du fichier bitmap)
            for (int i = 34; i < 38; i++)
            {
                returned[i] = tailleImageEndian[i - 34]; //puis on remplit dans la nouvelle image la taille de dans les octets correspondants (34 à 37)
            }

            int cpt = tailleOffset + 14; //on initialise un compte au premier octet de l'image c'est à dire à l'emplacement 14 (taille de l'entête du fichier bitmap) + la taille de l'offset
            int valeursInutiles = largeurImage % 4; //dans le cas ou nous sommes dans une image non multiple de 4 en largeur*hauteur
            for(int i = 0; i < hauteurImage; i++)
            {
                for(int j = 0; j < largeurImage; j++)
                {
                    returned[cpt] = Convert.ToByte(pixels[i, j].Blue); //on remplit chaque octet de l'image grace à notre matrice de pixel en prenant soin de l'ordre BGR
                    returned[cpt + 1] = Convert.ToByte(pixels[i, j].Green);
                    returned[cpt + 2] = Convert.ToByte(pixels[i, j].Red);
                    cpt += 3; //on passe au pixel suivant dans la nouvelle image en sautant les trois précédents octets
                }
                if(valeursInutiles != 0) //dans le cas ou nous sommes dans une image non multiple de 4 en largeur*hauteur
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

            File.WriteAllBytes(file, returned); //on écrit la nouvelle image bitmap dans le debug
        }

        public string toString() //fonction toString classique qui retourne les informations concernant l'image
        {
            string returned = "";
            returned += "Type de l'image : " + typeImage + "\n";
            returned += "Taille de l'image : " + Convert.ToString(tailleDuFichier) + "\n";
            returned += "Taille du header d'info : " + Convert.ToString(tailleOffset) + "\n";
            returned += "Largeur de l'image : " + Convert.ToString(largeurImage) + "\n";
            returned += "Hauteur de l'image : " + Convert.ToString(hauteurImage) + "\n";
            returned += "Nombre de bytes par couleur : " + Convert.ToString(nombreBytesParCouleur) + "\n"; 
            return returned;
        }

        public string AffichageMatricePixel() //méthode qui permet d'afficher notre matrice de pixel
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

        private string TypeDImage(byte[] image) //méthoque qui permet de savoir si l'image est du type bitmap ou autres
        {
            string returned = "";
            if (image[0] == 66 && image[1] == 77)
            {
                returned = "BM";
            }
            else returned = "Inconnu";
            return returned;
        }

        public int Convertir_Endian_To_Int(byte[] image, int nombreOctets, int indiceDepart)  //méthode qui permet de convertir un little endian en entier selon le nombre d'octet à partir d'un certain octet de du fichier image précisé dans les paramètres
        {
            int returned = 0;
            for(int x = 0; x < nombreOctets; x++)
            {
                returned += Convert.ToInt32(image[indiceDepart + x] * Math.Pow(256, x)); //on multiplie les octets les uns à la suite des autres avec la base 256 du little endian en incrémentant la puissance de 256 a chaque fois.
            }
            return returned;
        }

        public byte[] Convertir_Int_To_Endian(int val, int nombreOctets) //méthode inverse de la précédente
        {
            byte[] returned = new byte[nombreOctets]; //on prépare le tableau d'octet à retourner qui est da la taille demandé en paramètre
            for(int x = nombreOctets - 1; x >= 0; x--)
            {
                returned[x] = Convert.ToByte(val / Math.Pow(256, x)); 
                val = Convert.ToInt32(val % Math.Pow(256, x));
            }
            return returned;
        }
    }
}
