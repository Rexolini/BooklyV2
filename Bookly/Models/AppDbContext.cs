using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Bookly.Models
{
    public class AppDbContext : IdentityDbContext<Uzytkownik>
    {
        public AppDbContext() : base("CS", throwIfV1Schema: false){}

        public DbSet<Pisarz> Pisarze { get; set; }
        public DbSet<Etykieta> Etykiety { get; set; }
        public DbSet<Kategoria> Kategorie { get; set; }
        public DbSet<Wypozyczenie> Wypozyczenia { get; set; }
        public DbSet<Ksiazka> Ksiazki { get; set; }
        public DbSet<KsiazkaEtykieta> KsiazkaEtykiety { get; set; }
        public DbSet<KsiazkaPisarz> KsiakaPisarze { get; set; }
        public DbSet<SzukajArchiwum> Archiwa { get; set; }
        public DbSet<Plik> Pliki { get; set; }
        public DbSet<MagazynWartosc> MagazynWartosci { get; set; }
        

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }

        public System.Data.Entity.DbSet<Bookly.Models.Uzytkownik> Uzytkowniks { get; set; }
    }

    public static class ZarzadzanieTozsamoscia
    {
        public static UserManager<Uzytkownik> LokalnyUzytkownikZarzadzanie
        {
            get
            {
                return new UserManager<Uzytkownik>(new UserStore<Uzytkownik>(new AppDbContext()));
            }
        }

        public static RoleManager<IdentityRole> LokalnaRolaZarzadzanie
        {
            get
            {
                return new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new AppDbContext()));
            }
        }

        public static Uzytkownik GetUzytkownikById(string Id)
        {
            Uzytkownik uzytkownik = null;

            var uzytkownikZarzadzanie = LokalnyUzytkownikZarzadzanie;
            uzytkownik = uzytkownikZarzadzanie.FindById(Id);

            return uzytkownik;
        }

        public static bool UzytkownikIstniejeWRoliById(string Id, string role)
        {
            return LokalnyUzytkownikZarzadzanie.IsInRole(Id, role);
        }

        public static void UsunUzytkownikaZRoliById(string id, string role)
        {
            LokalnyUzytkownikZarzadzanie.RemoveFromRole(id, role);
        }

        public static void DodajUzytkownikaDoRoliById(string id, string role)
        {
            LokalnyUzytkownikZarzadzanie.AddToRole(id, role);
        }
        public static void StworzNowaRoleByName(string Imie)
        {
            var rola = new IdentityRole(Imie);
            LokalnaRolaZarzadzanie.Create(rola);
        }
    }
}