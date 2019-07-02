using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGameEngine
{
    public class Rysownik : Komponent
    {
        /******************** Właściwości i Pola******************/
        private int nrSprajtu = 0;
        public List<Image> Sprajty = new List<Image>();
        public int NrSprajtu
        {
            get {if (nrSprajtu < Sprajty.Count) return nrSprajtu;
                else return ((nrSprajtu-1) % Sprajty.Count); }
            set { nrSprajtu = value; }
        }
        public override string Nazwa //Zwracanie nazwy klasy
        {
            get { return "Rysownik"; }
        }
        /******************** Metody******************/
        public override void Renderuj()
        {
            if (nrSprajtu < 0) return;
            Image sprajt = Sprajty[NrSprajtu];
            int x = (int)ObiektyGryKomponentu.PozX;
            int y = (int)ObiektyGryKomponentu.PozY;
            SilnikGry.KameraGry.RysujObraz(sprajt, 0, 0, x, y, sprajt.Width, sprajt.Height, ObiektyGryKomponentu.Rotacja);
        }

        public override void Start() { }
        public override void Aktualizuj(float dt) { }
    }
}
