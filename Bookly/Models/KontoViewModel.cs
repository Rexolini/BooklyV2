using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bookly.Models
{
    public class ZewnetrznePotwierdzenieLogowaniaViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ZewnetrzaListaLogowaniaViewModel
    {
        public string ZwrocUrl { get; set; }
    }

    public class WyslijKodViewModel
    {
        public string SelectedProvider { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ZwrocUrl { get; set; }
        public bool ZapamietajMnie { get; set; }
    }

    public class WeryfikacjaKoduViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Kod")]
        public string Kod { get; set; }
        public string ZwrocUrl { get; set; }

        [Display(Name ="Zapamietaj tę przeglądarkę?")]
        public bool ZapamietajPrzegladarke { get; set; }
        public bool ZapamietajMnie { get; set; }
    }

    public class ZapomnijViewModel
    {
        [Required]
        [Display(Name ="Email")]
        public string Email { get; set; }
    }

    public class LogowanieViewModel
    {
        [Required]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Haslo")]
        public string Haslo { get; set; }

        [Display(Name = "Zapamietaj Mnie?")]
        public bool ZapamietajMnie { get; set; }
    }

    public class RejestracjaViewModel
    {
        [Required]
        [Display(Name ="Imie")]
        public string Imie { get; set; }

        [Required]
        [Display(Name = "Nazwisko")]
        public string Nazwisko { get; set; }

        [Required]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(12, ErrorMessage = "Haslo musi mieć od 6 do 12 znaków", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Haslo")]
        public string Haslo { get; set; }

        [Required]
        [Display(Name = "Potwierdz Haslo")]
        [Compare("Haslo", ErrorMessage = "Haslo musi byc takie same")]
        public string PotwierdzHaslo { get; set; }
    }

    public class ResetujHasloViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(12, ErrorMessage = "Haslo musi mieć od 6 do 12 znaków", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Haslo")]
        public string Haslo { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdz Haslo")]
        [Compare("Haslo", ErrorMessage = "Haslo musi byc takie same")]
        public string PotwierdzHaslo { get; set; }
        public string Kod { get; set; }
    }

    public class ZapomnialemHaslaViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name ="Email")]
        public string Email { get; set; }
    }
}