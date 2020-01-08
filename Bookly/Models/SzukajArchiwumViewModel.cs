using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookly.Models
{
    public class SzukajArchiwumViewModel
    {
        public SzukajArchiwumViewModel()
        {
            WybranaKategoria = 0;
            WybranyRok = 0;
        }
        public Ksiazka Ksiazka { get; set; }
        public IEnumerable<Ksiazka> Ksiazki { get; set; }
        public int[] WybranyPisarz { get; set; }
        public int[] WybraneEtykiety { get; set; }
        public int WybranaKategoria { get; set; }
        public int WybranyRok { get; set; }
        public string WybranyTytul { get; set; }
        public string WybranyISBN { get; set; }
        public string PisarzeOpcja { get; set; }
        public string EtykietyOpcja { get; set; }
        public string WszystkieOpcje { get; set; }
        public string ZapiszNazwe { get; set; }
        public bool TytulOpcja { get; set; }
        public bool KategoriaOpcja { get; set; }
        public bool RokOpcja { get; set; }
        public bool ISBNOpcja { get; set; }

        
    }
}