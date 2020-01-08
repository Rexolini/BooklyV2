using Bookly.App_Start;
using Bookly.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Bookly.Controllers
{
    [Authorize]
    public class ArchiwumController : Controller
    {
        private AppDbContext db = new AppDbContext();

        //GET: SzukajWArchiwum
        [Authorize(Roles ="Pracownik")]
        public ActionResult Index()
        {
            var szukajWArchiwum = db.Archiwa.Include(s => s.Czytelnik );
            return View(szukajWArchiwum.ToList());
        }
        public ActionResult UzytkownikIndex(string uzytkownikid)
        {
            var szukajWArchiwum = db.Archiwa.Where(a => a.Id_Czytelnika == uzytkownikid);
            return View(szukajWArchiwum.ToList());
        }

        //GET: SzuakjWArchiwum/Detale/5
        [Authorize(Roles ="Pracownik")]
        public ActionResult Detale(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            SzukajArchiwum szukajArchiwum = db.Archiwa.Find(id);
            if(szukajArchiwum == null)
            {
                return HttpNotFound();
            }
            return View(szukajArchiwum);
        }

        //GET: SzukajWArchiwum/Utworz
        [Authorize(Roles ="Pracownik")]
        public ActionResult Utworz()
        {
            ViewBag.CzytelnikId = new SelectList(db.Users, "Id", "Nazwa");
            return View();
        }

        //POST: SzukajWArchiwum/Utworz
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Pracownik")]
        public ActionResult Utworz([Bind(Include ="SzuakjArchiwumId, CzytelnikId, Nazwa, URL")] SzukajArchiwum szukajArchiwum)
        {
            if(ModelState.IsValid)
            {
                db.Archiwa.Add(szukajArchiwum);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CzytelnikId = new SelectList(db.Users, "Id", "Nazwa", szukajArchiwum.Id_Czytelnika);
            return View(szukajArchiwum);
        }

        //GET: SzukajWArchiwum/Edytuj/5
        [Authorize(Roles ="Pracownik")]
        public ActionResult Edytuj(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SzukajArchiwum szukajArchiwum = db.Archiwa.Find(id);
            if(szukajArchiwum == null)
            {
                return HttpNotFound();
            }
            ViewBag.CzytelnikId = new SelectList(db.Users, "Id", "Nazwa", szukajArchiwum.Id_Czytelnika);
            return View(szukajArchiwum);
        }

        //POST: SzukajWArchiwum/Edytuj/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Pracownik")]
        public ActionResult Edytuj([Bind(Include = "SzukajArchiwumId, CzytelnikId, Nazwa, URL")] SzukajArchiwum szukajArchiwum)
        {
            if(ModelState.IsValid)
            {
                db.Entry(szukajArchiwum).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CzytelnikId = new SelectList(db.Users, "Id", "Nazwa", szukajArchiwum.Id_Czytelnika);
            return View(szukajArchiwum);
        }

        //GET: SzukajWArchiwum/Usun/5
        [Authorize(Roles ="Pracownik")]
        public ActionResult Usun(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SzukajArchiwum szukajArchiwum = db.Archiwa.Find(id);
            if (szukajArchiwum == null)
            {
                return HttpNotFound();
            }
            return View(szukajArchiwum);
        }

        //POST: SzukajWArchiwum/Usun/5
        [HttpPost, ActionName("Usun")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Pracownik")]
        public ActionResult PotiwerdzenieUsuniecia(int id)
        {
            SzukajArchiwum szukajArchiwum = db.Archiwa.Find(id);
            db.Archiwa.Remove(szukajArchiwum);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}