using Bookly.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Bookly.Controllers
{
    [Authorize]
    public class UzytkownikController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Potwierdzenie(string id)
        {
            var uzytkownik = db.Users.FirstOrDefault(x => x.Id == id);
            return View(uzytkownik);
        }
        [HttpPost]
        public ActionResult Potwierdzenie(Uzytkownik uzytkownik)
        {
            var dbuzytkownik = db.Users.FirstOrDefault(x => x.Id == uzytkownik.Id);
            if (Request.Form["Usun"] != null)
            {
                db.Users.Remove(dbuzytkownik);
                db.SaveChanges();
            }
            else if (Request.Form["Potwierdzenie"] != null)
            {
                dbuzytkownik.EmailConfirmed = true;
                ZarzadzanieTozsamoscia.DodajUzytkownikaDoRoliById(User.Identity.GetUserId(), "Uzytkownik");
                db.SaveChanges();
            }
            return RedirectToAction("OczekiwanieNaResjestracje");
        }

        public ActionResult DodajDoRoli()
        {
            var uzytkownicy = db.Users.ToList();
            return View(uzytkownicy);
        }
        public ActionResult DodajUzytkownikaDoRoli(string id)
        {

            UzytkownikRolaView urvm = new UzytkownikRolaView();
            urvm.WszystkieRole = db.Roles.OrderBy(r => r.Name).Select(r => r.Name).ToList();
            urvm.Id_Uzytkownika = id;
            urvm.SprawdzRole = new bool[urvm.WszystkieRole.Count];
            for (int i = 0; i < urvm.WszystkieRole.Count; i++)
            {

                if (ZarzadzanieTozsamoscia.UzytkownikIstniejeWRoliById(id, urvm.WszystkieRole[i]))
                {
                    urvm.SprawdzRole[i] = true;
                }
                else
                {
                    urvm.SprawdzRole[i] = false;
                }
            }
            return View(urvm);
        }
        [HttpPost]
        public ActionResult DodajUzytkownikaDoRoli(UzytkownikRolaView urvm)
        {
            urvm.WszystkieRole = db.Roles.OrderBy(r => r.Name).Select(r => r.Name).ToList();
            for (int i = 0; i < urvm.SprawdzRole.Length; i++)
            {
                if (urvm.SprawdzRole[i])
                {
                    ZarzadzanieTozsamoscia.DodajUzytkownikaDoRoliById(urvm.Id_Uzytkownika, urvm.WszystkieRole[i]);
                }
                else
                {
                    ZarzadzanieTozsamoscia.UsunUzytkownikaZRoliById(urvm.Id_Uzytkownika, urvm.WszystkieRole[i]);
                }
            }
            return RedirectToAction("DodajDoRoli");
        }

        public ActionResult StworzRole()
        {
            var rola = db.Roles.OrderBy(r => r.Name).Select(r => r.Name).ToList();
            return View(rola);
        }

        public ActionResult StworzNowaRole()
        {
            return View();
        }
        [HttpPost]
        public ActionResult StworzNowaRole(string nazwa)
        {
            ZarzadzanieTozsamoscia.StworzNowaRoleByName(nazwa);
            return RedirectToAction("Stworzrole");
        }
        public ActionResult OczekiwanieRejestracji()
        {
            ICollection<Uzytkownik> uzytkownicy = db.Users.Where(x => x.EmailConfirmed == false).ToList();
            return View(uzytkownicy);
        }

        // GET: Uzytkownicy
        [Authorize(Roles = "Pracownik")]
        public ActionResult Index()
        {
            var uzytkownicy = db.Users.ToList();
            if (!User.IsInRole("Admin"))
            {
                uzytkownicy.Clear();
                var uzytkownikRola = db.Roles.FirstOrDefault(r => r.Name == "Uzytkownik");
                var adminRola = db.Roles.FirstOrDefault(r => r.Name == "Admin");
                var pracownikRola = db.Roles.FirstOrDefault(r => r.Name == "Pracownik");

                foreach (var uzytkownik in uzytkownikRola.Users.ToList())
                {
                    if (adminRola.Users.All(u => u.UserId != uzytkownik.UserId))
                    {
                        if (pracownikRola.Users.All(u => u.UserId != uzytkownik.UserId))
                        {
                            uzytkownicy.Add(db.Users.FirstOrDefault(u => u.Id == uzytkownik.UserId));
                        }
                    }
                }
            }

            return View(uzytkownicy);
        }

        // GET: Uzytkownicy/Detale/5
        public ActionResult Detale(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Uzytkownik uzytkownik = db.Users.Find(id);
            if (uzytkownik == null)
            {
                return HttpNotFound();
            }
            return View(uzytkownik);
        }

        // GET: Uzytkownicy/Edytuj/5
        public ActionResult Edytuj(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Uzytkownik uzytkownik = db.Users.Find(id);
            if (uzytkownik == null)
            {
                return HttpNotFound();
            }
            return View(uzytkownik);
        }

        // POST: Uzytkownicy/Edytuj/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edytuj([Bind(Include = "Id, Imie, Nazwisko, Email, EmailPotwierdzenie, " +
            "PasswordHash, SecurityStamp, NumerTelefonu, NumerTelefonuPotwierdzenie, TwoFactorEnabled, LockoutEndDateUtc," +
            "LockoutEnabled, AccessFailedCount,UserName")] Uzytkownik uzytkownik)
        {
            if (ModelState.IsValid)
            {
                db.Entry(uzytkownik).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(uzytkownik);
        }

        // GET: Uzytkownicy/Usun/5
        public ActionResult Usun(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Uzytkownik uzytkownik = db.Users.Find(id);
            if (uzytkownik == null)
            {
                return HttpNotFound();
            }
            return View(uzytkownik);
        }

        // POST: Uzytkownicy/Usun/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult UsunieciePotwierdzone(string id)
        {
            Uzytkownik uzytkownik = db.Users.Find(id);
            db.Users.Remove(uzytkownik);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}