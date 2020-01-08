using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookly.Models
{
    public class KsiazkaPisarz
    {
        public int Id_KsiazkaPisarz { get; set; }
        public int Id_Pisarz { get; set; }
        public int Id_Ksiazka { get; set; }
        public int Pozycja { get; set; }

        public virtual Pisarz Pisarz { get; set; }
        public virtual Ksiazka Ksiazka { get; set; }
    }
}