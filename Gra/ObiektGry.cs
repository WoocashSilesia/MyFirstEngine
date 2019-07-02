using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameEngine
{
    public class ObiektGry
    {
        /******************** Właściwości i obiekty******************/
        public string Nazwa { get; private set; }
        public bool Aktywny { get; set; }
        public Scena ScenaObiektuGry { get; set;}
        List<Komponent> Komponenty = new List<Komponent>();
        List<Komponent> KomponentyDoUsuniecia = new List<Komponent>();
        List<Komponent> KomponentyDoDodania = new List<Komponent>();
        // Pozycja na scenie obiektu
        private float rotacja;
        public float Rotacja
        {
            get { return rotacja; }
            set { rotacja = value % 360; }  //obrót o np.380 stopni to tak naprawde obrót o 20
        } //do przodu przy 0 st. to zwrot na północ, a kat liczony jest zgodnie ze wsk. zegara, np. +90* to na wschód
        public float PozX { get; set; }
        public float PozY { get; set; }

        /******************** Konstruktory ******************/
        public ObiektGry(string nazwa, float x = 0, float y = 0,float rotacja = 0, bool czyAktywny = true)
        {
            this.Nazwa = nazwa;
            PozX = x;
            PozY = y;
            this.rotacja = rotacja;
            Aktywny = czyAktywny;
        }

        /******************** Metody ******************/
        /******************** Dodaj/usuń/wyszukaj komponent ******************/
        public void DodajKomponent(Komponent komponent)
        {
            komponent.ObiektyGryKomponentu = this; //Dodatnie komponenty TEGO obiektu
            KomponentyDoDodania.Add(komponent);
        }
        public void UsunKomponent(Komponent komponent)
        {
            KomponentyDoUsuniecia.Add(komponent);
        }
        public Komponent DajKomponent(string nazwa, bool NieaktywneTez = false)
        {
            foreach (Komponent x in Komponenty)
            {
                if (string.Equals(x.Nazwa, nazwa) && (x.Aktywny || NieaktywneTez)) return x;
            }
            return null;
        }
        /******************** Start / aktualizuj / renduj ******************/
        internal void Start()
        {
            foreach (Komponent x in Komponenty) x.Start();
        }
        public void Aktualizuj(float dt)
        {
            //foreach (Komponent x in KomponentyDoDodania) Komponenty.Add(x);   //Bez tego od razu usuwamy cały obiekt wraz z wszystkimi jego komponentami. Do asteorid usuwamy wszystko,a nie część
            //KomponentyDoDodania.Clear();
            if(Aktywny) foreach (Komponent x in Komponenty) x.Aktualizuj(dt);
            foreach (Komponent x in KomponentyDoUsuniecia)
            {
                x.Aktywny = false;
                Komponenty.Remove(x);
            }
            KomponentyDoUsuniecia.Clear();
        }
        internal void Renderuj()
        {
            if(Aktywny) foreach (Komponent x in Komponenty) x.Renderuj();
        }
        /******************** Przemieszczanie ******************/
        public void DoPrzodu(float oIle)
        {
            PozX += (float)(oIle * Math.Sin(rotacja * Math.PI / 180));
            PozY -= (float)(oIle * Math.Cos(rotacja * Math.PI / 180));
        }
        public void WBok(float wPrawo)
        {
            PozX += (float)(wPrawo * Math.Sin((rotacja + 90) * Math.PI / 180));
            PozY -= (float)(wPrawo * Math.Cos((rotacja + 90) * Math.PI / 180));
        }
        public void Skrec(float oIleSkrec)
        {
            rotacja += oIleSkrec;
        }


        /*public List<Komponent> DajKomponenet_MojaWersja(string nazwa)
        {
            List<Komponent> DajListeKomponentow = new List<Komponent>();
            foreach (Komponent x in Komponenty)
            {
                if (string.Equals(x.Nazwa, nazwa) && x.Aktywny)
                {
                    DajListeKomponentow.Add(x);
                    return DajListeKomponentow
                }
            }
            return null;
        }
        */
    }
}
