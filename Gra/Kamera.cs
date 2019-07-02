using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameEngine
{
    public class Kamera
    {
        //pozycja kamery to lewy-górny róg
        //uklad wsp. skierowany jest do dołu i w prawo
        //kamera moze pokrywac caly bufor lub tylko jego fragment
        //przyjmujemy ze rozmiar bufora jest niemal nieskonczony
        //pivot rysowanych grafik znajduje sie w centrum grafiki
        /******************** Pola i Właściwości ******************/
        Image TylniBufor;
        Graphics Malarz;
        //
        public int X { get; private set; }
        public int Y { get; private set; }
        //Szerokosc i wysokosc kamery dopasowanej do sceny
        public int Szerokosc { get; private set; }
        public int Wysokosc { get; private set; }

        /******************** Konstruktor ******************/
        public Kamera(int szerokoscKamery, int wysokoscKamery, int szerBufora = 5000, int wysBufora = 4000)
        {
            Szerokosc = szerokoscKamery;
            Wysokosc = wysokoscKamery;
            TylniBufor = new Bitmap(szerBufora, wysBufora);
            Malarz = Graphics.FromImage(TylniBufor);
        }

        /******************** Konstruktor ******************/
        //Ustawianie pozycji kamery względem bufora ((X,Y) lewy górny róg)
        public void UstawPozycje(int x, int y)
        {
            if (x >= 0 && x + Szerokosc <= TylniBufor.Width) X = x;
            if (y >= 0 && y + Wysokosc <= TylniBufor.Height) Y = y;
        }
        public void WyczyscBufor(Image tlo = null)
        {
            if (tlo == null) Malarz.FillRectangle(Brushes.Yellow, new Rectangle(X, Y, Szerokosc, Wysokosc));
            else Malarz.DrawImageUnscaled(tlo, X, Y);
        }
        public void KopiujNaPrzedniBufor(Graphics przedniBufor, int matginesX = 0, int marginesY = 0)
        {
            /*przedni buffor jest bezposrednio od okna, 
            obszar z tylniego buffora wycinany jest na postawie X Y Wys Szer
            marginesy dotycza przedniego buffora, moge nie chciec kopiowac od 0 0*/
            Rectangle src = new Rectangle(X, Y, Szerokosc, Wysokosc);   //Struktura, która określa lokazlizacje i rozmiar do pobrania z tylniego bufora.
            Rectangle des = new Rectangle(X+matginesX, Y, Szerokosc+marginesY, Wysokosc); //Struktura, która określa lokalizacje i rozmiar do skopiowania na Przedni bufor. 
            przedniBufor.DrawImage(TylniBufor, des, src, GraphicsUnit.Pixel);
        }
        /******************** E10 ******************/
        public void RysujObraz(Image obraz, int xZ, int yZ, int xNa, int yNa, int szer, int wys, float rotacja)
        {
            if (rotacja == 0)
            {
                Rectangle des = new Rectangle((int)Math.Round(xNa - (szer -2f)), (int)Math.Round(yNa - (szer - 2f)), szer, wys); // 1 i 2 argument -> normalnie dawalibyśmy w lewym,górnym roku początek. Tak dajemy od razy w środku prostokąta
                Rectangle src = new Rectangle(xZ, yZ, szer, wys);
                Malarz.DrawImage(obraz, des, src, GraphicsUnit.Pixel);
            }
            else
            {
                Rectangle PoRotacji = ZnajdzObrysObrazu(szer, wys, rotacja);    //Nowy prostokatpo rotacji
                Image DoObrotu = new Bitmap(PoRotacji.Width, PoRotacji.Height);
                Graphics Obracanie = Graphics.FromImage(DoObrotu);  //doadawanie obrazu prostokat do "płótna" pomocnicznego
                Obracanie.TranslateTransform(DoObrotu.Width / 2, DoObrotu.Height / 2);  //oś wsp. na środek
                Obracanie.RotateTransform(rotacja); //obrót o zaany kąt
                Obracanie.TranslateTransform(-DoObrotu.Width / 2, -DoObrotu.Height / 2);    // powrót osi wsp.
                Rectangle des = new Rectangle((PoRotacji.Width / 2 - szer /2), (PoRotacji.Height/2 - wys /2), szer, wys);
                Rectangle src = new Rectangle(xZ, yZ, szer, wys);
                Obracanie.DrawImage(obraz, des, src, GraphicsUnit.Pixel);   //  wklejanie obróconego obrazu na płótno pomocnicze

                Rectangle WklenianieObrazuPoRotacji = new Rectangle((int)Math.Round(xNa - (szer - 2f)), (int)Math.Round(xNa - (szer - 2f)), PoRotacji.Width, PoRotacji.Height);
                Malarz.DrawImage(DoObrotu, WklenianieObrazuPoRotacji, PoRotacji, GraphicsUnit.Pixel);   //wklejanie obrazu na własciwe płótno
            }
        }
        public Rectangle ZnajdzObrysObrazu(int szerokosc, int wysokosc, float rotacja) //Szukamy obrysu wiekszego prostokąta, w który wpiszemy obrocony prostokat
        {
            float r = Math.Abs(rotacja) % 180; //Nieważne czy na górze, czy na dole. Tak samo wyglada prostokat
            if (r > 90) r = 180 - r;    //Nachylenie zostanie takie samo np. 30 st lub 150 > 90 -> 180 -150 = 30st. Otrzymujemy ten sam kąt
            float katAlfa = (float)(r * Math.PI / 180); //Kąt nachylenia
            szerokosc = (int)Math.Round(szerokosc * Math.Cos(katAlfa)) + (int)Math.Round(wysokosc * Math.Sin(katAlfa));
            wysokosc = (int)Math.Round(szerokosc * Math.Sin(katAlfa)) + (int)Math.Round(wysokosc * Math.Cos(katAlfa));
            Rectangle ObroconyProstokat = new Rectangle(0, 0, szerokosc, wysokosc);
            return ObroconyProstokat;
        }
        public void RysujTest(string tekst, int x, int y, Color kolor, int rozmiar, string czcionka="Arial")
        {
            Malarz.DrawString(tekst,new Font(new FontFamily(czcionka), rozmiar),new SolidBrush(kolor), x, y);
        }
        public void RysujProstokat(int x, int y, int szer, int wys, Color kolor)
        {
            Malarz.DrawRectangle(new Pen(kolor), x, y, szer, wys);
            // Malarz.FillRectangle(new SolidBrush(kolor), (int)Math.Round(x-szer/2f), (int)Math.Round(y - wys / 2f), szer, wys);   
        }
        public void RysujLinie(int xOd, int yOD, int xDo,int yDo, int grubosc, Color kolor)
        {
            Malarz.DrawLine(new Pen(kolor, grubosc), xOd, yOD, xDo, yDo);
        }

    }
}
