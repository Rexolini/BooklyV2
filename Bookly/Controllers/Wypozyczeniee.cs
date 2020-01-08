using Bookly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookly.Controllers
{
    public class Wypozyczeniee
    {
        private static readonly AppDbContext db = new AppDbContext();
        public static ICollection<string> StatusWypozyczenia = new List<string>() { "Ksiazka w magazynie", "Ksiazka gotowa do wysylki",
                                                                                    "Wypozyczona", "Zwrocona"};
        public static bool JestAktualnieWypozyczona(int ksiazkaid, string uzytkownikid)
        {
            var wypozyczona = db.Wypozyczenia.Where(w => w.Id_Czytelnika == uzytkownikid && w.Id_Ksiazka == ksiazkaid).ToList();
            if(wypozyczona.Count > 0)
            {
                return false;
            }
            return true;
        }

        public static bool MozeZostacWypozyczona(int ksiazkaid)
        {
            var ksiazkanaklad = db.Ksiazki.FirstOrDefault(k => k.Id_Ksiazka == ksiazkaid).Naklad;
            var wypozyczoneksiazki = db.Wypozyczenia.Where(w => w.Id_Ksiazka == ksiazkaid && w.ZwrotData < w.WypozyczenieData).ToList();
            if(wypozyczoneksiazki.Count < ksiazkanaklad)
            {
                return true;
            }

            return false;
        }

        public static bool CzyCzytelnikMozeWypozyczyc(string uzytkownikid)
        {
            var maxwypozyczen = db.MagazynWartosci.Find(1).MaksymalneWypozyczenia;
            var wypozyczeniauzytkownika = db.Wypozyczenia.Where(w => w.ZwrotData < w.WypozyczenieData && w.Id_Czytelnika == uzytkownikid).ToList();
            if(wypozyczeniauzytkownika.Count < maxwypozyczen || maxwypozyczen == 0)
            {
                return true;
            }

            return false;
        }
    }
}