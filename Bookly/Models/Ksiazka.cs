using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bookly.Models
{
    public class Ksiazka
    {
        public int Id_Ksiazka { get; set; }
        public int Id_Kategorii { get; set; }
        public string Tytul { get; set; }
        public string ISBN { get; set; }
        public int Rok { get; set; }
        public int Naklad { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DodajDate { get; set; }
        public string Opis { get; set; }
        public string Zawartosc { get; set; }

        public virtual Kategoria Kategoria { get; set; }
        public virtual ICollection<KsiazkaEtykieta> Etykiety { get; set; }
        public virtual ICollection<KsiazkaPisarz> Pisarze { get; set; }
        public virtual IEnumerable<Plik> Pliki { get; set; }
        public virtual IEnumerable<Wypozyczenie> Wypozyczenia { get; set; }
    }
}