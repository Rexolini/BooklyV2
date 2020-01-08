using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bookly.Models
{
    public class KategoriaViewModel
    {
        public int Id_Kategorii { get; set; }
        [Required]
        public string Nazwa { get; set; }

        public int? Id_PoZaKategoria { get; set; }
    }
}