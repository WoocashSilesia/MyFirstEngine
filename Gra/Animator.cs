using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGameEngine
{
    public class Animator : Komponent
    {
        /******************** Właściwości i Pola******************/
        public override string Nazwa { get { return "Animator"; } }
        public List<Animacja> Animacje = new List<Animacja>();
        public event EventHandler<Animacja> ZakonczonoAnimacje;
   
        public Animacja ObecnaAnimacja { get; private set; }
        Animacja NastepnaAnimacja;
        int nrObecnejKlatki;
        bool zapetlObecnaAnimacje;
        DateTime czasOstatnijKlatki;
        bool zapetlNastepnaAnimacje;
        bool wykonujAnimcaje;
        /******************** Metody ******************/


        public override void Aktualizuj(float dt)
        {
            if (!wykonujAnimcaje) return;  //Wyjdź jak nie ma być animacji
            if ((DateTime.Now - czasOstatnijKlatki).TotalSeconds < ObecnaAnimacja.CzasNaKlatke) return; //wyjdź jeżeli czas, który upłynął jest mniejszy niż czas wykonywania klatki
            if(nrObecnejKlatki < ObecnaAnimacja.IleKlatek-1) //Dajemy -1, z tego powodu, że jak jest 10 klatek i jesteśmy na 8, wchodzimy do petli i DAJEMY 9 klatke, czyli przedostatnią. Na ostatnią kaltke jest inny if
            {
                nrObecnejKlatki++;  //przejdź do nastepnej klatki
                czasOstatnijKlatki = DateTime.Now; //ustaw czas wykonywania klatki
            }
            else
            {
                ZakonczonoAnimacje?.Invoke(this, ObecnaAnimacja);       //Wywołaj event o zakonczeniu animacji (przechodzi ostatnia klatka)
                if(NastepnaAnimacja == null) //Jeżeli po obecnej animacji nie mamy następnej
                {
                    if(zapetlObecnaAnimacje)    //Opcja 1: Zapetlamy obecna animacje
                    {
                        nrObecnejKlatki = 0;
                        czasOstatnijKlatki = DateTime.Now;
                    }
                    else  //Wychodzimy
                    {
                        if (ObecnaAnimacja.KonczNaPierwszejKlatca) nrObecnejKlatki = 0; //mozemy na koniec wyswietlic pierwsza klatke
                        wykonujAnimcaje = false;
                    }
                }
                else
                {//Przerzucanie nastęnej klatki na obecną
                    ObecnaAnimacja = NastepnaAnimacja;
                    nrObecnejKlatki = 0;
                    zapetlObecnaAnimacje = zapetlNastepnaAnimacje;
                    NastepnaAnimacja = null;
                    czasOstatnijKlatki = DateTime.Now;
                }
            }
        }

        public override void Renderuj()
        {
            if (ObecnaAnimacja == null) return;
            Image sprajt = ObecnaAnimacja.Atlas;
            //Ustawianie X i Y : wybieramy współrzędne x i y jak ma być animacja w poziomie to punkt przesuwamy o szerokosc prostokąta pomnożona przez numer obecnaj klatki. Tak samo z pionem
            int klatkaX = ObecnaAnimacja.PierwszaKlatka.X + (ObecnaAnimacja.AnimacjaWPionie ? 0 : ObecnaAnimacja.PierwszaKlatka.Width * nrObecnejKlatki);
            int klatkaY = ObecnaAnimacja.PierwszaKlatka.Y + (ObecnaAnimacja.AnimacjaWPionie ? ObecnaAnimacja.PierwszaKlatka.Height * nrObecnejKlatki : 0);
            int gdzieWklejX = (int)ObiektyGryKomponentu.PozX;
            int gdzieWklejY = (int)ObiektyGryKomponentu.PozY;
            Rectangle klatka = new Rectangle(klatkaX, klatkaY, ObecnaAnimacja.PierwszaKlatka.Width, ObecnaAnimacja.PierwszaKlatka.Height);
            SilnikGry.KameraGry.RysujObraz(sprajt, klatkaX, klatkaY, gdzieWklejX, gdzieWklejY, klatka.Width, klatka.Height, ObiektyGryKomponentu.Rotacja);
        }

        public override void Start()
        {
            if(Animacje.Count > 0 && wykonujAnimcaje == false)
            {
                ObecnaAnimacja = Animacje.First(); // Zwraca pierwszy element z Listy Animacje
                nrObecnejKlatki = 0;
            }
        }
        public void UruchomAplikacje(string Nazwa, bool poSkonczeniuObecnej = false, bool zapetlic=false)
        {
            Animacja a = null;
            foreach(Animacja x in Animacje)
            {
                if (string.Equals(Nazwa, x.Nazwa))
                {
                    a = x;
                    break;
                }
            }
            if (a == null) return; //jak nie znalazlem takiej animacji to nic nie rob
            if (poSkonczeniuObecnej && wykonujAnimcaje) //Czekam na skonczenie obecnej animacji (dodanie do kolejki)
            {
                NastepnaAnimacja = a;
                zapetlNastepnaAnimacje = zapetlic;
            }
            else //jesli mam natychmiast rozpoczac ta animacje lub i tak nic innego nie animuje
            {
                AnulujAnimacje();
                ObecnaAnimacja = a;
                zapetlObecnaAnimacje = zapetlic;
                czasOstatnijKlatki = DateTime.Now;
                nrObecnejKlatki = 0;
                wykonujAnimcaje = true;
            }
        }
        public void AnulujAnimacje()
        {
            if (wykonujAnimcaje) nrObecnejKlatki = (ObecnaAnimacja.KonczNaPierwszejKlatca ? 0 : ObecnaAnimacja.IleKlatek - 1); 
            wykonujAnimcaje = false;
            NastepnaAnimacja = null;
        }
    }
}
