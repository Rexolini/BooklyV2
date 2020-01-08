using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookly.Models
{
    public class SzukajArchiwum
    {
        public int Id_Archiwum { get; set; }
        public string Id_Czytelnika { get; set; }
        public string Nazwa { get; set; }
        public string URL { get; set; }

        public virtual Uzytkownik Czytelnik { get; set; }
    }
}