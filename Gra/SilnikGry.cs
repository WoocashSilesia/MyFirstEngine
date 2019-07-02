using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms; //Do dziedziczenia po Form

namespace MyGameEngine
{
    public class SilnikGry : Form
    {
        //Pola i właściości
        static SilnikGry Silnik;
        static List<Keys> WcisnieteKlawisze = new List<Keys>();

        public static Scena ObecnaScena { get; private set; }
        public static Scena KolejnaScena { get; private set; }
        public static Kamera KameraGry { get; private set; }
        public static float SkalarCzasu { get; set; }

        /******************** Konstruktry ******************/
        private SilnikGry()
        {
            SkalarCzasu = 1f;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.Opaque, true);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SilnikGry_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SilnikGry_KeyUp);
            Silnik = this;
        }
        /******************** Tryb okienkowy ******************/
        public SilnikGry(string tytul, int szerokosc, int wysokosc) : this()
        {
            this.ClientSize = new System.Drawing.Size(szerokosc, wysokosc);
            this.Text = tytul;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }
        /******************** Tryb pełnoekranowy ******************/
        public SilnikGry(string tytul)
        {
            this.Text = tytul;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        /******************** Key_Down i Key_Up ******************/
        private void SilnikGry_KeyDown(object sender, KeyEventArgs e)
        {
            if (!WcisnieteKlawisze.Contains(e.KeyCode)) WcisnieteKlawisze.Add(e.KeyCode);   //Dodaje do  Listy WcisnieteKlawisze jak jest wciśniety klawisz
        }

        private void SilnikGry_KeyUp(object sender, KeyEventArgs e)
        {
            WcisnieteKlawisze.Remove(e.KeyCode);    //Usuwa klawisz jak przestaje być wcisniety
        }


        /******************** Metody statyczne  ******************/

        public static void ZaladujScene(Scena nowaScena) 
        {
            KolejnaScena = nowaScena;           //Ma zapamiętac przekazaną w argumencie scene
        }

        //Sprawdzenie wszystkich rodzajów klawiszy przekazanych w argumentach i zwrócenie true, jeśli wszystkie z nich 
        //Znajdują się w Liście
        public static bool Klawisz(params Keys[] klawisze)
        {
            foreach (Keys x in klawisze)
            {
                if (!WcisnieteKlawisze.Contains(x)) return false;
            }
            return true;
        }

        //Zakończenie gry
        public static void ZakonczGre()
        {
            Silnik.Close();
        }
        /******************** Pętla Gry ******************/

        public void Graj(Scena ScenaPoczatkowa, int maxFPS = 30)
        {
            //Inicjalizacja skłądników gry
            KameraGry = new Kamera(this.ClientSize.Width, this.ClientSize.Height); //Stworzenie obiektu kamery
            ZaladujScene(ScenaPoczatkowa);    //Załadowanie początkowej sceny
            this.Show();   //pokazanie okna aplikacji

            //Obiekt klasy Graphics możemy utworzyć poprzez kilka jej statycznych metod
            //lub poprzez metodę CreateGraphics() dla kontrolek WinForms
            Graphics przedniBufor = Graphics.FromHwnd(this.Handle);     //Obszar graficzny naszego okna. Nasze "płótno" przednie 
            //Danie uchytu do naszej aplikacji okienkowej 
            DateTime czasOstatnijeKlatki = DateTime.Now;    //Dzięki znajomości czasu obliczam FPS
            double czasNaKlatke = 1000.0 / maxFPS;  //Tyle milisekund mamy na stworzenie 1 klatki
            Image tlo = Bitmap.FromFile("Atlasy/background.png");       //Wstawianie tła do okna jako tło do czyszczenia bufora

            while (true)    //petla gry
            {
                if (!this.Created) break;   //Wyjście z pętli gry
                if (KolejnaScena != null)   //Jak zmieniamy scene np. nowy poziom
                {
                    KameraGry.WyczyscBufor();
                    ObecnaScena = KolejnaScena;
                    KolejnaScena = null;
                    ObecnaScena.Start();
                }
                float dt = (float)(DateTime.Now - czasOstatnijeKlatki).TotalSeconds;    //Czas jaki upłynął od ostatniej klatki na scenie
                czasOstatnijeKlatki = DateTime.Now;     //Kolejne dt będzie liczone względem tej chwili

                ObecnaScena.Aktualizuj(dt * SkalarCzasu);   //Obiekty na scenie aktualizują swój stan
                KameraGry.WyczyscBufor(tlo);    //Czyszczenie obrazu z poprzednij klatki                
                ObecnaScena.Renderuj(); //Rysowanie obiektów na ObecnaScena
                KameraGry.KopiujNaPrzedniBufor(przedniBufor);  //Gotową klatkę wklejamy na ekran

                Application.DoEvents(); //Odpowiada za responsywnosć okna, np. wcisniecia klawiszy
                double przerwa = czasNaKlatke - (DateTime.Now - czasOstatnijeKlatki).TotalMilliseconds;
                if (przerwa > 0) System.Threading.Thread.Sleep((int)przerwa);   //Jeżeli scena wykonała się szybciej niż czas na 1 klatke, to zrób przerwe
            }

        }

    }
}
