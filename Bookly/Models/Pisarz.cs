using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bookly.Models
{
    public class Pisarz
    {
        public int Id_Pisarza { get; set; }
        [Required]
        public string Imie { get; set; }
        [Required]
        public string Nazwisko { get; set; }

        public virtual IEnumerable<KsiazkaPisarz> KsiazkaPisarze { get; set; }
    }
}