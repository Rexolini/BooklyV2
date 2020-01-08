using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bookly.Models
{
    public class Wypozyczenie
    {
        public int Id_Wypozyczenia { get; set; }
        public int Id_Ksiazka { get; set; }
        public string Id_Czytelnika { get; set; }
        public string Status { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM=dd}", ApplyFormatInEditMode =true)]
        public DateTime WypozyczenieData { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM=dd}", ApplyFormatInEditMode = true)]
        public DateTime ZwrotData { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM=dd}", ApplyFormatInEditMode = true)]
        public DateTime OstatecznyCzas { get; set; }

        public virtual Uzytkownik Czytelnik { get; set; }
        public virtual Ksiazka Ksiazka { get; set; }
    }
}