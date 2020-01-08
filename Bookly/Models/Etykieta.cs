using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bookly.Models
{
    public class Etykieta
    {
        public int Id_Etykieta { get; set; }
        [Required]
        public string Nazwa { get; set; }

        public virtual IEnumerable<KsiazkaEtykieta> KsiazkaEtykiety { get; set; }
    }
}