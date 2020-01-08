using Bookly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Bookly.Controllers
{
    public class EtykietyController : Controller
    {
        private AppDbContext db = new AppDbContext();

        //GET: Etykiety
        public ActionResult Index()
        {
            return View(db.Etykiety.ToList());
        }

        //GET: Etykiety/Detale/5
        public ActionResult Detale(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Etykieta etykieta = db.Etykiety.Find(id);
            if(etykieta == null)
            {
                return HttpNotFound();
            }
            return View(etykieta);
        }

        //GET: Etykiety/Stworz
        public ActionResult Stworz()
        {
            return View();
        }

        //POST: Etykiety/Stworz
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Stworz([Bind(Include = "EtykietaId, Nazwa")] Etykieta etykieta)
        {
            if(ModelState.IsValid)
            {
                db.Etykiety.Add(etykieta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(etykieta);
        }

        //GET: Etykiety/Edytuj/5
        public ActionResult Edytuj(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Etykieta etykieta = db.Etykiety.Find(id);
            if(etykieta == null)
            {
                return HttpNotFound();
            }
            return View(etykieta);
        }

        //POST: Etykiety/Edytuj/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edytuj([Bind(Include = "EtykietaId, Nazwa")] Etykieta etykieta)
        {
            if(ModelState.IsValid)
            {
                db.Entry(etykieta).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(etykieta);
        }

        //GET: Etykiety/Usun/5
        public ActionResult Usun(int? id)
        {
            if( id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Etykieta etykieta = db.Etykiety.Find(id);
            if(etykieta == null)
            {
                return HttpNotFound();
            }
            return View(etykieta);
        }

        //POST: Etykiety/Usun/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult PotwierdzenieUsuniecia(int id)
        {
            Etykieta etykieta = db.Etykiety.Find(id);
            db.Etykiety.Remove(etykieta);
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