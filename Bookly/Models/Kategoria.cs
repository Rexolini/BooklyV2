using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bookly.Models
{
    public class Kategoria
    {
        public int Id_Kategoria { get; set; }
        [Required]
        public string Nazwa { get; set; }
        [Display(Name ="Po za Kategoria")]
        [Required]
        public int Id_PoZaKategoria { get; set; }

        public virtual IEnumerable<Ksiazka> Ksiazki { get; set; }
        public virtual Kategoria PoZaKategoria { get; set; }
    }
}