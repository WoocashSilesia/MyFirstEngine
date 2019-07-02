using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameEngine
{
    public abstract class Komponent
    {
        /******************** Właściwości i pola******************/
        public abstract string Nazwa { get; }
        public bool Aktywny { get; set; }
        public ObiektGry ObiektyGryKomponentu { get; set; } //metody start, aktualizuj, renderuj
        public abstract void Start();
        public abstract void Aktualizuj(float dt);
        public abstract void Renderuj();
        /******************** Konstruktor ******************/
        public Komponent()
        {
            Aktywny = true;
        }
    }
}
