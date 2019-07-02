using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGameEngine
{
    public class Kolider : Komponent
    {
        /******************** Właściwości i Pola******************/
        public override string Nazwa
        {
            get { return "Kolider"; }
        }
        List<Point> Punkty = new List<Point>();     //Punkty kreslące wielokąt względem srodka grafiki obiektu
        public int warstwaFizyczna = 0;
        public bool czyWyrysowac { get; set; }
        public event EventHandler<Kolider> WejscieWKolizje;
        public event EventHandler<Kolider> WyjscieZKolizji;
        List<Kolider> KolidujaceKolidery = new List<Kolider>();
        /******************** Metody******************/

        public override void Aktualizuj(float dt)
        {
            //sprawdz klizje z kazdym z moich i rzuc event jesli zachodzi
            foreach (Kolider x in SilnikGry.ObecnaScena.DajKomponent(Nazwa))  //Sprawdz kolizje dla kazdego aktywnego komponentu na senie
            {               
                Kolider kol1 = (Kolider)x; //Rzutowanie komponentu na klasę kolidera
                if (kol1.warstwaFizyczna != warstwaFizyczna || kol1 == this) continue; //tylko kolidery w tej samej warstwie mogą kolidowac
                if (SprawdzKolizje(kol1))
                {
                    InformujOKolizji(kol1, true);      //informuje siebie o kolizji
                    kol1.InformujOKolizji(this, true);   //informuje drugi obiekt o kolizji
                }
                else
                {
                    InformujOKolizji(kol1, false);   //informuje siebie o braku kolizji
                    kol1.InformujOKolizji(this, false);  //informuje drugi obiekt o braku kolizji
                }

                for (int i = 0; i < KolidujaceKolidery.Count; i++)
                {
                   Kolider kol2  = KolidujaceKolidery[i];
                   if(kol2.ObiektyGryKomponentu == null || !kol2.Aktywny)
                    {
                        KolidujaceKolidery.Remove(kol2);
                        i--;
                        WyjscieZKolizji?.Invoke(this, kol2);
                    }
                }
            }
        }
        /******************** Sprawdzanie kolizji + metody pomocnicze******************/
        bool SprawdzKolizje(Kolider kol)
        {
            if (warstwaFizyczna != kol.warstwaFizyczna || this == kol) return false;//sam ze soba ani w innej warstwie nie koliduj
            for (int i = 0; i < Punkty.Count; i++)//dla kazdego z odcinokow tego kolidera
            {
                Point p1 = PunktyPoTransformacji(Punkty[i]);
                Point p2 = PunktyPoTransformacji(Punkty[(i + 1) % Punkty.Count]);
                for (int j = 0; j < kol.Punkty.Count; j++)  //sprawdz z kazdym odcinkiem innego kolidera
                {
                    Point p3 = PunktyPoTransformacji(kol.Punkty[i]);
                    Point p4 = PunktyPoTransformacji(kol.Punkty[(i + 1) % kol.Punkty.Count]);
                    if (CzySiePrzecinaja(p1, p2, p3, p4)) return true;  //jak sie przecinaja to musza kolidowac
                }
            }
            return false;    //jak sie odnicki nie przecinaly to sa to calkiem rozlaczne wielokaty
        }
        Point PunktyPoTransformacji(Point p)
        {
            float px = ObiektyGryKomponentu.PozX;
            float py = ObiektyGryKomponentu.PozY;
            float kat = ObiektyGryKomponentu.Rotacja;
            double xPoRot = p.X * (Math.Cos(kat) * Math.PI / 180) - p.Y * (Math.Sin(kat) * Math.PI / 180);
            double yPoRot = p.X * (Math.Sin(kat) * Math.PI / 180) + p.Y * (Math.Cos(kat) * Math.PI / 180);
            return new Point((int)xPoRot, (int)yPoRot);
        }
        private bool CzySiePrzecinaja(Point A, Point B, Point C, Point D)
        {
            float v1 = IloczynWektorowy(A, B, C);
            float v2 = IloczynWektorowy(A, B, D);
            float v3 = IloczynWektorowy(C, D, A);
            float v4 = IloczynWektorowy(C, D, B);
            if (((v1 > 0 && v2 < 0) || (v1 < 0 && v2 > 0)) && ((v3 > 0 && v4 < 0) || (v3 < 0 && v4 > 0))) return true;
            //sprawdzenie, czy koniec odcinka leży na drugim
            if (v1 == 0 && SprawdzCzyLezyNa(A, B, C)) return true;
            if (v2 == 0 && SprawdzCzyLezyNa(A, B, D)) return true;
            if (v3 == 0 && SprawdzCzyLezyNa(C, D, A)) return true;
            if (v4 == 0 && SprawdzCzyLezyNa(C, D, B)) return true;
            return false;
        }
        private float IloczynWektorowy(Point A, Point B, Point C)
        {
            float x1 = B.X - A.X;
            float y1 = B.Y - A.Y;
            float x2 = C.X - A.X;
            float y2 = C.Y - A.Y;
            return x1 * y2 - x2 * y1;
        }
        bool SprawdzCzyLezyNa(Point A, Point B, Point C) //sprawdzenie, czy punkt C(koniec odcinka pierwszego) leży na odcinku AB
        {
            return Math.Min(A.X, B.X) <= C.X && C.X <= Math.Max(A.X, B.X)
                && Math.Min(A.Y, B.Y) <= C.Y && C.Y <= Math.Max(A.Y, B.Y);
        }

        /******************** Informacje o kolizji******************/
        private void InformujOKolizji(Kolider zKim, bool czyZderzenie)
        {
            if (czyZderzenie) //Jezlei zderzenie jest na true
            {
                if (!KolidujaceKolidery.Contains(zKim)) //Sprawdzamy, czy wczesniej nie kolidował
                {
                    KolidujaceKolidery.Add(zKim);   //Dodajemy obiekt do listy koliderów
                    WejscieWKolizje?.Invoke(this, zKim);//event mówiący o wejsciu w kolizje
                }
            }
            else //Jezeli nie ma kolizji obiektów
            {
                if (KolidujaceKolidery.Contains(zKim)) //sprawdzamy czy kolider znajduje się w liście Kolidującyhc
                {
                    KolidujaceKolidery.Remove(zKim); //Usuwamy kolidera, bo to oznacza, że juz przestał koilidować
                    WyjscieZKolizji?.Invoke(this, zKim);  //event o wyjściu z kolizji
                }
            }
        }


        public override void Renderuj()
        {
            if(czyWyrysowac)
            {
                for (int i = 0; i < Punkty.Count; i++)
                {
                    Point p1 = PunktyPoTransformacji( Punkty[i]);
                    Point p2 = PunktyPoTransformacji( Punkty[(i + 1) % Punkty.Count]);
                    SilnikGry.KameraGry.RysujLinie(p1.X, p1.Y, p2.X, p2.Y, 2, Color.Green);
                }
            }
        }

        public override void Start() { }

        public void DodajProstokat(int x, int y, int szer, int wys) // xy to lewy górny róg
        {
            Punkty.Add(new Point(x, y));
            Punkty.Add(new Point(x + szer, y));
            Punkty.Add(new Point(x + szer, y + wys));
            Punkty.Add(new Point(x, y + szer));
        }
    }
}
