using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookly.Models
{
    public class KsiazkaEtykieta
    {
        public int Id_KsiazkiEtykiety { get; set; }
        public int Id_Ksiazki { get; set; }
        public int Id_Etykiety { get; set; }

        public virtual Ksiazka Ksiazka { get; set; }
        public virtual Etykieta Etykieta { get; set; }
    }
}