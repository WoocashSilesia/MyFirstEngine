using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameEngine
{
    public class Scena
    {
        /******************** Właściwości i obiekty******************/
        List<ObiektGry> Obiekty = new List<ObiektGry>();
        List<ObiektGry> ObiektyDoUsuniecia = new List<ObiektGry>();
        List<ObiektGry> ObiektyDoDodania = new List<ObiektGry>();
        public DateTime CzasRozpoczecia { get; private set; }

        /******************** Metody ******************/
        public virtual void Start()
        {
            CzasRozpoczecia = DateTime.Now;
            foreach (ObiektGry x in Obiekty)    //Każdy obiekt zaczyna "sam" się rysować na scenie
            {
                x.Start();
            }
        }
        public virtual void Aktualizuj(float dt)    //W aktualizacji dodajemy obiekty, aktualizujemy je,a nastepnie usuwamy
        {
            foreach (ObiektGry x in ObiektyDoDodania)
            {
                Obiekty.Add(x);
            }
            ObiektyDoDodania.Clear(); //Czyszczenie Listy obiektów do dodania, po ich dodaniu
            foreach (ObiektGry x in Obiekty)
            {
                x.Aktualizuj(dt);
            }
            foreach (ObiektGry x in ObiektyDoUsuniecia)
            {
                x.Aktywny = false;  //Zmien obiekt na niekatywny
                Obiekty.Remove(x);
            }
            ObiektyDoUsuniecia.Clear();
        }
        public virtual void Renderuj()
        {
            foreach (ObiektGry x in Obiekty)
            {
                x.Renderuj();
            }
        }
        /******************** Dodawanie/Usuwanie i Wyszukowanie obiektu/komponentu ******************/
        public void DodajObiekt(ObiektGry Obiekt)
        {
            Obiekt.ScenaObiektuGry = this;  //Chcemy obiekt dodać do NASZEJ sceny obiekty, dlatego przypisujemy do this (do aktualnej sceny)
            ObiektyDoDodania.Add(Obiekt);
        }
        public void UsunObiekt(ObiektGry Obiekt)
        {
            ObiektyDoUsuniecia.Add(Obiekt);
        }
        public ObiektGry DajObiekt(string nazwa, bool NieaktywneTez = false) // Daj obiekt - nieaktywnetez -> jeśli 
        {
            foreach (ObiektGry x in Obiekty)
            {
                if (string.Equals(x.Nazwa, nazwa) && (x.Aktywny || NieaktywneTez)) return x;
            }
            return null;
        }
        public List<ObiektGry> DajObiektyPodobneDo(string nazwaCzesciowa, bool NieaktywneTez = false)
        {
            List<ObiektGry> ListaObiektowPodobnychDo = new List<ObiektGry>();
            foreach (ObiektGry x in Obiekty)
            {
                if(x.Nazwa.Contains(nazwaCzesciowa) && (x.Aktywny || NieaktywneTez))
                {
                    ListaObiektowPodobnychDo.Add(x);
                }
            }
            return ListaObiektowPodobnychDo;
        }
        public List<Komponent> DajKomponent(string nazwa, bool NieaktywneTez = false)
        {
            List<Komponent> ListaKomponentow = new List<Komponent>();
            foreach(ObiektGry x in Obiekty)
            {
                Komponent k = x.DajKomponent(nazwa);
                if (k != null && ((k.Aktywny && x.Aktywny) || NieaktywneTez)) ListaKomponentow.Add(k);
            }
            return ListaKomponentow;
        }



        /*public List<ObiektGry> DajObiekt_MojaWersja(string nazwa)
        {
            List<ObiektGry> DajListeObiektow = new List<ObiektGry>();
            foreach (ObiektGry x in Obiekty)
            {
                if (string.Equals(x.Nazwa, nazwa) && x.Aktywny)
                {
                    DajListeObiektow.Add(x);
                    return DajListeObiektow;
                }
            }
            return null;
        }
        */
    }
}
