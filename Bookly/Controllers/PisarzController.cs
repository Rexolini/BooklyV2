using Bookly.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Bookly.Controllers
{
    [Authorize(Roles = "Pracownik")]
    public class PisarzController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Pisarze
        public ActionResult Index()
        {
            return View(db.Pisarze.ToList());
        }

        // GET: Pisarze/Detale/5
        public ActionResult Detale(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pisarz writer = db.Pisarze.Find(id);
            if (writer == null)
            {
                return HttpNotFound();
            }
            return View(writer);
        }

        // GET: Pisarze/Utworz
        public ActionResult Utworz()
        {
            return View();
        }

        // POST: Pisarze/Utworz
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PisarzId, Imie, Nazwisko")] Pisarz pisarz)
        {
            if (ModelState.IsValid)
            {
                db.Pisarze.Add(pisarz);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pisarz);
        }

        // GET: Pisarze/Edytuj/5
        public ActionResult Edytuj(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pisarz pisarz = db.Pisarze.Find(id);
            if (pisarz == null)
            {
                return HttpNotFound();
            }
            return View(pisarz);
        }

        // POST: Pisarze/Edytuj/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PisarzId, Imie, Nazwisko")] Pisarz pisarz)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pisarz).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pisarz);
        }

        // GET: Pisarze/Usun/5
        public ActionResult Usun(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pisarz pisarz = db.Pisarze.Find(id);
            if (pisarz == null)
            {
                return HttpNotFound();
            }
            return View(pisarz);
        }

        // POST: Pisarze/Usun/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult PotwierdzenieUsuniecia(int id)
        {
            Pisarz pisarz = db.Pisarze.Find(id);
            db.Pisarze.Remove(pisarz);
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