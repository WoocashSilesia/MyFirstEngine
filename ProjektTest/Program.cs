using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyGameEngine;
using System.Drawing;

namespace ProjektTest
{
    static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Rysownik r = new Rysownik();
            r.Sprajty.Add(Bitmap.FromFile("Atlasy/Opis.png"));

            ObiektGry o = new ObiektGry("Tlo",0,0);
            o.DodajKomponent(r);

            Scena s = new Scena();
            s.DodajObiekt(o);

            new SilnikGry("Test",1600,1200).Graj(s, 30);
        }
    }
}
