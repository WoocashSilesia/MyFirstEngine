using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGameEngine
{
    public class Animacja
    {
        /******************** Właściwości i Pola******************/
        public string Nazwa { get; private set; }
        public Image Atlas { get; private set; }
        public Rectangle PierwszaKlatka { get; private set; }
        public int IleKlatek { get; private set; }
        public float CzasTrwania { get; private set; }
        public bool AnimacjaWPionie { get; private set; }    //F: z lewej na prawą, T:z góry na dół
        public bool KonczNaPierwszejKlatca { get; private set; }    //moze na koniec wyswietlic pierwsza klatke
        public float CzasNaKlatke { get { return CzasTrwania / IleKlatek; } }
        /******************** Konstruktor ******************/
        public Animacja(string nazwa, Image atlas, Rectangle klatka, int ileKlatek, float czas, bool wPionie = false, bool konczNaPierwszej = false)
        {
            this.Nazwa = nazwa;
            this.Atlas = atlas;
            this.PierwszaKlatka = klatka;
            this.IleKlatek = ileKlatek;
            this.CzasTrwania = czas;
            this.AnimacjaWPionie = wPionie;
            this.KonczNaPierwszejKlatca = konczNaPierwszej;
        }
    }
}
