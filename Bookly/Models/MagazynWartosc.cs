using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bookly.Models
{
    public class MagazynWartosc
    {
        public int Id_Magazyn { get; set; }
        [Range(0, 10)]// okresla maksymalna ilosc ksiazek mozliwych do wypozyczenia w jednym momencie
        public int MaksymalneWypozyczenia { get; set; }
        [Range(0, Int32.MaxValue)]// okresla jak dlugo mozna trzymac ksiazki
        public int MaksymalnyCzasWypozyczenia { get; set; }
        [Range(0, 5)]// okresla czas jaki czytelnik ma na odbior ksiazki z oddzialu
        public int CzasNaOdbior { get; set; }
        [Range(0, 3)]// okresla maksymalna ilosc ksiazek do wypozyczenia przy jednym odbiorze
        public int MaksymalnaIloscKsiazekDoOdebrania { get; set; }
    }
}