using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Bookly.App_Start;
using Bookly.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;


namespace Bookly.Controllers
{
    [Authorize]
    public class WypozyczenieController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult UzytkownikIndex(string uzytkownikid)
        {
            var uzytkownik = ZarzadzanieTozsamoscia.GetUzytkownikById(uzytkownikid);
            var wypozyczenia = db.Wypozyczenia.Where(b => b.Id_Czytelnika == uzytkownik.Id);
            return View(wypozyczenia.ToList());
        }

        //GET: Wypozyczenia
        public ActionResult Index()
        {
            var wypozyczenia = db.Wypozyczenia.Include(x => x.Ksiazka).Include(w => w.Czytelnik);
            return View(wypozyczenia.ToList());
        }

        //GET: Wypozyczenia/Detale/5
        public ActionResult Detale(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wypozyczenie wypozyczenie = db.Wypozyczenia.Find(id);
            if(wypozyczenie == null)
            {
                return HttpNotFound();
            }
            return View(wypozyczenie);
                
        }

        //GET: Wypozyczenie/Utworz
        [Authorize(Roles = "Pracownik")]
        public ActionResult Utworz()
        {
            ViewBag.Id_Ksiazki = new SelectList(db.Ksiazki, "KsiazkiId", "Tytuly");
            ViewBag.Id_Czytelnika = new SelectList(db.Users, "Id", "Imie");
            return View();
        }

        //POST: Wypozyczenia/Utworz
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Pracownik")]
        public ActionResult Utworz([Bind(Include ="WypozyczenieId, KsiazkaId, CzytelnikId, DataZwrotu, OstatecznaData, Status")] Wypozyczenie wypozyczenie)
        {
            if(ModelState.IsValid)
            {
                db.Wypozyczenia.Add(wypozyczenie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            IEnumerable<SelectListItem> czytelnicy = from c in db.Users
                                                     select new SelectListItem
                                                     {
                                                         Value = c.Id,
                                                         Text = c.Imie + " " + c.Nazwisko + " " + "[" + c.Email + "]"
                                                     };
            ViewBag.KsiazkaId = new SelectList(db.Ksiazki, "KsiazkaId", "Tytul", wypozyczenie.Id_Ksiazka);
            ViewBag.CzytelnikId = new SelectList(czytelnicy, "Value", "Text", wypozyczenie.Id_Czytelnika);
            return View(wypozyczenie);
        }

        // GET: Wypozyczenie/Edytuj/5
        [Authorize(Roles = "Pracownik")]
        public ActionResult Edytuj(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wypozyczenie wypozyczenie = db.Wypozyczenia.Find(id);
            if (wypozyczenie == null)
            {
                return HttpNotFound();
            }
            IEnumerable<SelectListItem> czytelnicy = from c in db.Users
                                                  select new SelectListItem
                                                  {
                                                      Value = c.Id,
                                                      Text = c.Imie + " " + c.Nazwisko + "[" + c.Email + "]"
                                                  };
            ViewBag.KsiazkaId = new SelectList(db.Ksiazki, "KsiazkaId", "Tytul", wypozyczenie.Id_Ksiazka);
            ViewBag.CzytelnikId = new SelectList(czytelnicy, "Value", "Text", wypozyczenie.Id_Czytelnika);
            ViewBag.ListaStatusu = new SelectList(Wypozyczeniee.StatusWypozyczenia);
            return View(wypozyczenie);

        }

        // POST: Wypozyczenie/Edytuj/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Pracownik")]
        public async Task<ActionResult> Edytuj([Bind(Include = "WypozyczenieId, KsiazkaId," +
                                                " CzytelnikId, DataZwrotu, OstatecznaData, Status")] Wypozyczenie wypozyczenie)
        {

            if (ModelState.IsValid)
            {
                db.Entry(wypozyczenie).State = EntityState.Modified;
                db.SaveChanges();
                if (ViewBag.previuosstatus == null)
                {
                    await WyslijMaila(wypozyczenie.Id_Czytelnika, wypozyczenie.Status);
                }
                else if (ViewBag.previuosstatus != wypozyczenie.Status)
                {
                    await WyslijMaila(wypozyczenie.Id_Czytelnika, wypozyczenie.Status);
                }
                return RedirectToAction("Index");
            }
            IEnumerable<SelectListItem> czytelnicy = from c in db.Users
                                                  select new SelectListItem
                                                  {
                                                      Value = c.Id,
                                                      Text = c.Imie + " " + c.Nazwisko + "[" + c.Email + "]"
                                                  };
            ViewBag.KsiazkaId = new SelectList(db.Ksiazki, "KsiazkaId", "Tytul", wypozyczenie.Id_Ksiazka);
            ViewBag.CzytelnikId = new SelectList(czytelnicy, "Value", "Text", wypozyczenie.Id_Czytelnika);
            ViewBag.ListaStatusu = new SelectList(Wypozyczeniee.StatusWypozyczenia);
            ViewBag.poprzednistatus = wypozyczenie.Status;
            return View(wypozyczenie);
        }

        private async Task WyslijMaila(string uzytkownikId, string status)
        {
            UserManager<Uzytkownik> uzytkownikZrzadzanie =
                        HttpContext.GetOwinContext().GetUserManager<AplikacjaUzytkownikZarzadzanie>();
            await uzytkownikZrzadzanie.SendEmailAsync(uzytkownikId, "Status Twojego zamowienia uległ zmianie", "Aktualny Status: " + status);
        }
        // GET: Wypozyczenie/Usun/5
        [Authorize(Roles = "Pracownik")]
        public ActionResult Usun(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wypozyczenie wypozyczenie = db.Wypozyczenia.Find(id);
            if (wypozyczenie == null)
            {
                return HttpNotFound();
            }
            return View(wypozyczenie);
        }

        // POST: Wypozyczenie/Usun/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Pracownik")]
        public ActionResult PotwierdzenieUsuniecia(int id)
        {
            Wypozyczenie wypozyczenie = db.Wypozyczenia.Find(id);
            db.Wypozyczenia.Remove(wypozyczenie);
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
