using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Bookly.Models
{
    public class Uzytkownik : IdentityUser
    {
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Miasto { get; set; }
        public string Adres { get; set; }
        public string Kod_Pocztowy { get; set; }
        
        public virtual IEnumerable<Wypozyczenie> Wypozyczenia { get; set; }
        public virtual IEnumerable<SzukajArchiwum> SzukajArchiwa { get; set; }

        public async Task<ClaimsIdentity> GenerujIdUzytkownikaAsync(UserManager<Uzytkownik> administrator)
        {
            var Id_Uzytkownika = await administrator.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return Id_Uzytkownika;
        }
    }
}