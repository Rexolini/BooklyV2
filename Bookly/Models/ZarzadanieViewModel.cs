using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bookly.Models
{
    public class ZarzadanieViewModel
    {
        public bool PosiadaHaslo { get; set; }
        public IList<UserLoginInfo> Loginy { get; set; }
        public string NumerTelefonu { get; set; }
        public bool Fuzja { get; set; }
        public bool ZapamietajPrzegladarke { get; set; }
    }

    public class ZarzadzajLoginamiViewModel
    {
        public IEnumerable<UserLoginInfo> AktualneLoginy { get; set; }
        public IEnumerable<AuthenticationDescription> InneLoginy { get; set; }
    }

    public class UstawHasloViewModel
    {
        [Required]
        [StringLength(12, ErrorMessage = "Hasło musi mieć miedzy 6, a 12 znaków", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nowe Haslo")]
        public string NoweHaslo { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdz nowe haslo")]
        [Compare("NoweHaslo", ErrorMessage = "Hasła nie zgadzają się ze sobą")]
        public string PotwierdzenieHaslo { get; set; }
    }

    public class ZmienHasloViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Aktualne Haslo")]
        public string StareHaslo { get; set; }

        [Required]
        [StringLength(12, ErrorMessage = "Hasło musi mieć miedzy 6, a 12 znaków", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nowe Haslo")]
        public string NoweHaslo { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdz nowe haslo")]
        [Compare("NoweHaslo", ErrorMessage = "Hasła nie zgadzają się ze sobą")]
        public string PotwierdzenieHasla { get; set; }
    }

    public class DodajNumerTelefonuViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Numer Telefonu")]
        public string Numer { get; set; }
    }

    public class ZweryfikujNumerTelefonuViewModel
    {
        [Required]
        [Display(Name = "Kod")]
        public string Kod { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Numer Telefonu")]
        public string NumerTelefonu { get; set; }
    }

    public class KonfiguracjaFuzjiViewModel
    {
        public string SelectedProvider { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}