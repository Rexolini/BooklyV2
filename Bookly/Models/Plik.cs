using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookly.Models
{
    public class Plik
    {
        public int Id_Plik { get; set; }
        public int Id_Ksiazka { get; set; }
        public string Zrodlo { get; set; }
        public string Nazwa { get; set; }

        public virtual Ksiazka Ksiazka { get; set; }
    }
}