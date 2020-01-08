using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookly.Models
{
    public class Koszyk
    {
        public Koszyk()
        {
            Ksiazki = new List<Ksiazka>();
        }
        public int KoszykID { get; set; }
        public virtual ICollection<Ksiazka> Ksiazki { get; set; }
    }
}