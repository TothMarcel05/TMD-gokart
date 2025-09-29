using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TMD_gokart
{
    internal class Program
    {
        #region Versenyzo
        class Versenyzo
        {
            public string VNev { get; set; }
            public string KNev { get; set; }
            public DateTime SzIdo { get; set; }
            public string Elmult18 { get; set; }
            public string VAzon { get; set; }
            public string Email { get; set; }

            public Versenyzo(string vnev, string knev, DateTime szido, string elmult)
            {
                VNev = vnev;
                KNev = knev;
                SzIdo = szido;
                Elmult18 = elmult;
                VAzon = AzonGen(vnev, knev, szido);
                Email = EmailGen(vnev, knev);
            }

            private string EkezetNelkul(string szoveg)
            {
                string eredeti = "ÁÉÍÓÖŐÚÜŰáéíóöőúüű";
                string csere = "AEIOOOUUUaeiooouuu";
                for (int i = 0; i < eredeti.Length; i++)
                {
                    szoveg = szoveg.Replace(eredeti[i], csere[i]);
                }
                return szoveg;
            }

            private string EmailGen(string vnev, string knev)
            {
                string teljesNev = (vnev + "." + knev).ToLower();
                teljesNev = EkezetNelkul(teljesNev).Replace(" ", "");
                return $"{teljesNev}@gmail.com";
            }

            private string AzonGen(string vnev, string knev, DateTime szido)
            {
                string teljesNev = EkezetNelkul(vnev + knev).Replace(" ", "");
                return $"GO-{teljesNev}-{szido:yyyyMMdd}";
            }

            public override string ToString()
            {
                return $"{VNev} {KNev}, {SzIdo:yyyy.MM.dd}, Elmúlt-e 18: {Elmult18}, {VAzon}, {Email}";
            }
        }
        #endregion

        #region Foglalás kezelés
        static void FoglalasKezeles(List<Versenyzo> versenyzok, Dictionary<DateTime, List<string>> foglalasok)
        {
            Console.WriteLine("\nElérhető versenyzők:");
            foreach (var v in versenyzok)
            {
                Console.WriteLine(v.VAzon);
            }

            Console.Write("\nAdd meg a kiválasztott versenyző azonosítóját: ");
            string azon = Console.ReadLine();

            Versenyzo versenyzo = versenyzok.Find(x => x.VAzon == azon);
            if (versenyzo == null)
            {
                Console.WriteLine("Nincs ilyen azonosító!");
                return;
            }

            Console.Write("Add meg a foglalás dátumát (ÉÉÉÉ.HH.NN): ");
            DateTime datum = DateTime.Parse(Console.ReadLine());

            Console.Write("Add meg a kezdő órát (8-18): ");
            int ora = int.Parse(Console.ReadLine());

            Console.Write("Add meg a foglalás hosszát (1 vagy 2 óra): ");
            int hosszusag = int.Parse(Console.ReadLine());

            for (int i = 0; i < hosszusag; i++)
            {
                DateTime idopont = new DateTime(datum.Year, datum.Month, datum.Day, ora + i, 0, 0);
                if (!foglalasok.ContainsKey(idopont))
                    foglalasok[idopont] = new List<string>();

                if (foglalasok[idopont].Count < 20) 
                {
                    foglalasok[idopont].Add(versenyzo.VAzon);
                    Console.WriteLine($"Foglalás rögzítve: {idopont:yyyy.MM.dd HH}:00-{idopont.AddHours(1):HH}:00");
                }
                else
                {
                    Console.WriteLine($"A {idopont:HH-mm} időpont betelt!");
                }
            }
        }
        #endregion

        #region Naptár megjelenítés
        static void IdopontokMegjelenitese(Dictionary<DateTime, List<string>> foglalasok)
        {
            Console.WriteLine("\nIdőpontok a hónap végéig:");
            Console.Write("\t\t");
            for (int ora = 8; ora < 19; ora++)
            {
                Console.Write($"\t{ora}-{ora + 1}");
            }
            Console.WriteLine();

            int napokSzama = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            for (int nap = DateTime.Now.Day; nap <= napokSzama; nap++)
            {
                Console.ResetColor();
                Console.Write($"{DateTime.Now.Year}.{DateTime.Now.Month:D2}.{nap:D2}\t");

                for (int i = 8; i < 19; i++)
                {
                    DateTime idopont = new DateTime(DateTime.Now.Year, DateTime.Now.Month, nap, i, 0, 0);
                    if (foglalasok.ContainsKey(idopont) && foglalasok[idopont].Count > 0)
                    {
                        Console.BackgroundColor = ConsoleColor.Red; 
                        Console.Write("\tFoglalt");
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Green; 
                        Console.Write("\tSzabad");
                    }
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }
        #endregion

        static void Main(string[] args)
        {
            #region 0. feladat
            Console.WriteLine("TMD, 2025.09.15, Gokart időpontfoglaló - Egyéni kisprojekt");
            #endregion
            #region 1. feladat
            string gnev = "Y Ddraig Goch Gokartpálya";
            string gcim = "6120 Kiskunmajsa, Rózsa u 7.";
            string gtszam = "+36-30-911-2001";
            string gwebd = "Y-Ddraig-Goch-gokart.hu";
            Console.WriteLine($"{gnev}; {gcim}; {gtszam}; {gwebd}");
            #endregion
            #region 2. feladat
            string VNevSor = File.ReadAllText("vezeteknevek.txt", Encoding.UTF8);
            string KNevSor = File.ReadAllText("keresztnevek.txt", Encoding.UTF8);

            string[] VNvekek = VNevSor.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(n => n.Trim().Trim('\'')).ToArray();

            string[] KNevek = KNevSor.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(n => n.Trim().Trim('\'')).ToArray();

            Random rnd = new Random();
            DateTime ma = DateTime.Now;
            int versenyzoDb = rnd.Next(1, 151);

            List<Versenyzo> versenyzok = new List<Versenyzo>();
            for (int i = 0; i < versenyzoDb; i++)
            {
                string vnev = VNvekek[rnd.Next(VNvekek.Length)];
                string knev = KNevek[rnd.Next(KNevek.Length)];

                int ev = rnd.Next(1900, ma.Year + 1);
                int ho = rnd.Next(1, 13);
                int nap = rnd.Next(1, DateTime.DaysInMonth(ev, ho) + 1);
                DateTime szido = new DateTime(ev, ho, nap);

                bool erett = (ma.Year - szido.Year > 18) ||
                             (ma.Year - szido.Year == 18 && ma.DayOfYear >= szido.DayOfYear);
                string elmult = erett ? "Igen" : "Nem";

                Versenyzo uj = new Versenyzo(vnev, knev, szido, elmult);
                versenyzok.Add(uj);
            }

            Console.WriteLine($"\nGenerált versenyzők száma: {versenyzoDb}\n");
            foreach (var item in versenyzok.Take(10)) // csak első 10, hogy ne legyen túl hosszú
            {
                Console.WriteLine(item);
            }
            #endregion
            #region 3–5. feladat
            Dictionary<DateTime, List<string>> foglalasok = new Dictionary<DateTime, List<string>>();
            while (true)
            {
             
                Console.WriteLine("\nVálassz műveletet:");
                Console.WriteLine("1 - Időpontok megjelenítése");
                Console.WriteLine("2 - Foglalás rögzítése (versenyző azonosító alapján)");
                Console.WriteLine("0 - Kilépés");
                Console.Write("Választás: ");
                string valasz = Console.ReadLine();
                Console.Clear();
                if (valasz == "1") IdopontokMegjelenitese(foglalasok);
                else if (valasz == "2") FoglalasKezeles(versenyzok, foglalasok);
                else if (valasz == "0") break;
                else Console.WriteLine("Érvénytelen választás!");
               
            }
            #endregion
        }
    }
}
