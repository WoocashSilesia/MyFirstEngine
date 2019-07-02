using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyGameEngine
{
    public class Dzwiek : Komponent
    {
        /******************** Właściwości i Pola******************/
        public override string Nazwa { get { return "Dzwiek"; } }
        public ObiektGry Sluchacz = null;   //Będziemy skalowac dzwiek w zaloznosci od odległosci od niego
        public float odlegloscMax = 0; //Dalej niżmax zawsze nic nie słychać
        public float odlegloscMin = 0; // bliżej niż min, zawsze słychać 100%
        List<MediaPlayer> Grajkowie = new List<MediaPlayer>();

        /******************** Metody******************/
        public override void Start() { }
        public override void Renderuj() { }
        public override void Aktualizuj(float dt)
        {
            for (int i = 0; i < Grajkowie.Count; i++)
            {
                if (Grajkowie[i].Position == Grajkowie[i].NaturalDuration)  //jesli dotarlismy do konca odtwarzania
                {
                    Grajkowie.RemoveAt(i);  //pozbywamy sie grajka
                    i--;    //zmniejszylismy rozmiar kolekcji o 1 po niej iterujemy, elementy spadna o 1 pozycje, wiec odejmujemy o 1, aby zacząć poprawnie odczytywać następną iteracje
                }
                else Grajkowie[i].Volume = SkalujGloscnosc();
            }
        }
        /******************** Ustawianie słuchacza  i granie******************/
        public void UstawSluchacza(ObiektGry sluchacz, float odlegloscMin, float odlegloscMax)
        {
            this.Sluchacz = sluchacz;
            this.odlegloscMax = odlegloscMax;
            this.odlegloscMin = odlegloscMin;
        }
        public void Graj(string Nazwa, float predkosc = 0.99f)
        {
            if(Sluchacz == null || !Sluchacz.Aktywny) //jak nie ma sluchacza to gramy niezaleznie np. melodyjka w tle
            {
                GrajBezSkalowania(Nazwa,0.99f,predkosc);
                return;
            }
            MediaPlayer mp = new MediaPlayer();
            mp.Open(new System.Uri(System.IO.Directory.GetCurrentDirectory() + "\\" + Nazwa));
            mp.SpeedRatio = predkosc;
            mp.Volume = SkalujGloscnosc();
            mp.Play();
            Grajkowie.Add(mp); // Jeżeli jest słuchacz to tworzymy nowy MediaPlayer
        }

        public void GrajBezSkalowania(string Nazwa, float gloscnosc= 0.99f, float predkosc = 1f)
        {
            MediaPlayer mp = new MediaPlayer();
            mp.Open(new System.Uri(System.IO.Directory.GetCurrentDirectory() + "\\" + Nazwa));
            mp.SpeedRatio = predkosc;
            mp.Volume = gloscnosc;
            mp.Play();
        }
        /******************** Sklaowanie głoscności******************/
        float SkalujGloscnosc() {
            if (Sluchacz == null || !Sluchacz.Aktywny) return 0.99f;
            double odleglosc = Math.Sqrt(Math.Pow((Sluchacz.PozX - ObiektyGryKomponentu.PozX), 2.0) 
                + Math.Pow(Sluchacz.PozY - ObiektyGryKomponentu.PozY, 2)); //Pitagoras, odl Słuchacza od drugiego obiektu
            if (odleglosc > odlegloscMax) return 0f;
            else if (odleglosc < odlegloscMin) return 0.99f;
            else return (float)(1 - (odleglosc - odlegloscMin) / (odlegloscMax - odleglosc));
        }
        /******************** Zatrzymaj dzwiek******************/
        public void ZatrzymajDzwieki()
        {
            foreach (MediaPlayer x in Grajkowie)
            {
                x.Stop();
                Grajkowie.Clear();      //??????????????
            }
        }
    }
}
